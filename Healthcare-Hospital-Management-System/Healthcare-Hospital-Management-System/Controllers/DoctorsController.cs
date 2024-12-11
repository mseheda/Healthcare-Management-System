using HealthcareHospitalManagementSystem.Infrastructure;
using HealthcareHospitalManagementSystem.Models;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

[Authorize]
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

    [HttpGet("SerializeDeserializeTest")]
    public ActionResult SerializeDeserializeTest()
    {
        var serializer = new AppSerializer();

        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-CA");
        var now = DateTime.Now;
        double number = 1234.56;

        var serializedDate = serializer.SerializeDateTime(now);
        var serializedNumber = serializer.SerializeNumber(number);

        _logger.Log($"[fr-CA] Serialized DateTime: {serializedDate}");
        _logger.Log($"[fr-CA] Serialized Number: {serializedNumber}");

        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
        var deserializedDate = serializer.DeserializeDateTime(serializedDate);
        var deserializedNumber = serializer.DeserializeNumber<double>(serializedNumber);

        _logger.Log($"[de-DE] Deserialized DateTime: {deserializedDate}");
        _logger.Log($"[de-DE] Deserialized Number: {deserializedNumber}");

        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        _logger.Log($"[en-US] Final Result: DateTime: {deserializedDate}, Number: {deserializedNumber}");

        var montrealTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var laTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

        var montrealDateTime = DateTime.SpecifyKind(deserializedDate, DateTimeKind.Unspecified);

        var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(montrealDateTime, montrealTimeZone);

        var laDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, laTimeZone);

        _logger.Log($"Converted DateTime from Montreal (GMT-4) to LA (GMT-8): {laDateTime}");

        return Ok("Serialization/Deserialization test completed");
    }
}
