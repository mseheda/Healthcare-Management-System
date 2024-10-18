using HealthcareHospitalManagementSystem.Infrastructure;
using HealthcareHospitalManagementSystem.Models;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly ICustomLogger _logger;
    private readonly INotificationService _notificationService;

    public DoctorsController(IDoctorService doctorService, ICustomLogger logger, INotificationService notificationService)
    {
        _doctorService = doctorService;
        _logger = logger;
        _notificationService = notificationService;

        if (_doctorService is DoctorService service)
        {
            service.Logger = (Logger)logger;
            service.Logger.Log("DoctorService instance created.");
        }
    }

    [HttpPost]
    public ActionResult AddDoctor(Doctor doctor)
    {
        try
        {
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
}
