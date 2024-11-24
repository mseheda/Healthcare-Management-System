namespace Healthcare_Hospital_Management_System.Models
{
    public class AddTimeSlotRequest
    {
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public DateOnly SlotDate { get; set; }
        public TimeOnly SlotTime { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
