using Healthcare_Hospital_Management_System.Models;
using HealthcareHospitalManagementSystem.Models;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DoctorAppointmentController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorAppointmentController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpPost("AddTimeSlot")]
    public ActionResult AddTimeSlot([FromBody] AddTimeSlotRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request.");
        }

        var clientDateTime = request.SlotDate.ToDateTime(request.SlotTime);

        var serverDateTime = TimeZoneHelper.ConvertClientTimeToServerTime(clientDateTime, request.ClientTimeZoneId);

        var success = _doctorService.AddTimeSlot(
            request.DoctorName,
            request.Specialization,
            serverDateTime,
            TimeSpan.FromMinutes(request.DurationInMinutes));

        if (success)
        {
            return Ok("Time slot added successfully.");
        }

        return BadRequest("Failed to add time slot. The doctor may not exist, or the time slot may already be added.");
    }



    [HttpGet("AvailableDoctors")]
    public ActionResult<List<Doctor>> GetAvailableDoctors(
        [FromQuery] string specialization,
        [FromQuery] DateTime date,
        [FromQuery] TimeSpan? time = null,
        [FromQuery] string clientTimeZoneId = "Eastern Standard Time")
    {
        DateTime clientDateTime;
        if (time.HasValue)
        {
            clientDateTime = new DateTime(date.Year, date.Month, date.Day, time.Value.Hours, time.Value.Minutes, time.Value.Seconds);
        }
        else
        {
            clientDateTime = date;
        }

        var serverDateTime = TimeZoneHelper.ConvertClientTimeToServerTime(clientDateTime, clientTimeZoneId);

        var doctors = _doctorService.GetAvailableDoctors(
            specialization,
            serverDateTime,
            time.HasValue ? serverDateTime.TimeOfDay : (TimeSpan?)null);

        if (!doctors.Any())
        {
            return NotFound($"No available doctors found for specialization {specialization} on {serverDateTime:yyyy-MM-dd}" +
                            (time.HasValue ? $" at {serverDateTime:HH:mm}" : ""));
        }

        return Ok(doctors);
    }

    [HttpPost("Schedule")]
    public ActionResult ScheduleAppointment([FromBody] ScheduleAppointmentRequest request)
    {
        if (request == null)
        {
            return BadRequest("Invalid request.");
        }

        var clientDateTime = request.AppointmentDate.ToDateTime(request.AppointmentTime);

        var serverDateTime = TimeZoneHelper.ConvertClientTimeToServerTime(clientDateTime, request.ClientTimeZoneId);

        string timeOfDay = ClassifyAppointmentTime(serverDateTime, request.IsHoliday, request.IsWeekend);

        if (_doctorService.ScheduleAppointment(request.DoctorName, request.Specialization, serverDateTime, request.DurationInMinutes))
        {
            return Ok($"Appointment scheduled successfully for the {timeOfDay}.");
        }

        return BadRequest("Failed to schedule appointment. The doctor may not be available at the specified date and time.");
    }

    private string ClassifyAppointmentTime(DateTime dateTime, bool isHoliday, bool isWeekend)
    {
        if (isHoliday)
        {
            return "Holiday";
        }

        if (isWeekend)
        {
            return dateTime.Hour switch
            {
                >= 6 and < 12 => "Weekend Morning",
                >= 12 and < 18 => "Weekend Afternoon",
                >= 18 and <= 22 => "Weekend Evening",
                _ => "Unavailable on Weekends"
            };
        }

        return dateTime.Hour switch
        {
            >= 5 and < 12 => "Morning",
            >= 12 and < 17 => "Afternoon",
            >= 17 and < 21 => "Evening",
            >= 21 and < 24 => "Late Night Emergency",
            _ => "Unavailable"
        };
    }

}