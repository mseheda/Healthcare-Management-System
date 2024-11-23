using Healthcare_Hospital_Management_System.Models;
using HealthcareHospitalManagementSystem.Models;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

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

        var slotDateTime = request.SlotDate.ToDateTime(request.SlotTime);

        var success = _doctorService.AddTimeSlot(
            request.DoctorName,
            request.Specialization,
            slotDateTime,
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
    [FromQuery] TimeSpan? time = null)
    {
        var doctors = _doctorService.GetAvailableDoctors(specialization, date, time);

        if (!doctors.Any())
        {
            return NotFound($"No available doctors found for specialization {specialization} on {date:yyyy-MM-dd}" +
                            (time.HasValue ? $" at {time}" : ""));
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

        var appointmentDateTime = request.AppointmentDate.ToDateTime(request.AppointmentTime);

        if (_doctorService.ScheduleAppointment(request.DoctorName, request.Specialization, appointmentDateTime, request.DurationInMinutes))
        {
            return Ok("Appointment scheduled successfully.");
        }

        return BadRequest("Failed to schedule appointment. The doctor may not be available at the specified date and time.");
    }
}
