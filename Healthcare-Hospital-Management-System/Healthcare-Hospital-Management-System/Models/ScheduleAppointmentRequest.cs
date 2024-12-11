namespace Healthcare_Hospital_Management_System.Models
{
    public class ScheduleAppointmentRequest
    {
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly AppointmentTime { get; set; }
        public int DurationInMinutes { get; set; }
        public string ClientTimeZoneId { get; set; } = "Eastern Standard Time";
    }
}
