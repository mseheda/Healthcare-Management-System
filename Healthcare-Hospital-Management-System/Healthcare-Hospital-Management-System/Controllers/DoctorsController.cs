using HealthcareHospitalManagementSystem.Infrastructure;
using HealthcareHospitalManagementSystem.Models;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly IHealthcareLogger _logger;
    private readonly INotificationService _notificationService;

    public DoctorsController(IDoctorService doctorService, IHealthcareLogger logger, INotificationService notificationService)
    {
        _doctorService = doctorService;
        _logger = logger;
        _notificationService = notificationService;

        if (_doctorService is DoctorService service)
        {
            service.Logger = (HealthcareLogger)logger;
            service.Logger.Log("DoctorService instance created.");
        }
    }

    [HttpPost]
    public ActionResult AddDoctor([FromQuery] string name = "John Doe",
                                  [FromQuery] int age = 45,
                                  [FromQuery] string specialization = "General Medicine",
                                  [FromQuery] string department = "Internal Medicine")
    {
        try
        {
            var doctor = new Doctor
            {
                Name = name,
                Age = age,
                Specialization = specialization,
                Department = department
            };

            _doctorService.AddDoctor(doctor, (NotificationService)_notificationService);
            return Ok();
        }
        catch (ObjectDisposedException)
        {
            return StatusCode(500, "Error logging transaction");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("AddTimeSlot")]
    public ActionResult AddTimeSlot(
    [FromQuery] string doctorName,
    [FromQuery] string specialization,
    [FromQuery] DateTime date,
    [FromQuery] TimeSpan time)
    {
        if (_doctorService.AddTimeSlot(doctorName, specialization, date, time))
        {
            return Ok("Time slot added successfully.");
        }

        return BadRequest("Failed to add time slot. The doctor may not exist, or the time slot may already be added.");
    }

    [HttpGet]
    public ActionResult<List<Doctor>> GetDoctors()
    {
        try
        {
            var doctors = _doctorService.GetDoctors();
            if (!doctors.Any())
                return NotFound("No doctors found");

            return Ok(doctors);
        }
        catch (ObjectDisposedException)
        {
            return StatusCode(500, "Service is disposed");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
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
    public ActionResult ScheduleAppointment(
    [FromQuery] string doctorName,
    [FromQuery] string specialization,
    [FromQuery] DateTime date,
    [FromQuery] TimeSpan time)
    {
        if (_doctorService.ScheduleAppointment(doctorName, specialization, date, time))
        {
            return Ok("Appointment scheduled successfully.");
        }

        return BadRequest("Failed to schedule appointment. The doctor may not be available on the selected date and time.");
    }
}
