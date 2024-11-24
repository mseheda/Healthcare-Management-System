namespace Healthcare_Hospital_Management_System.Models
{
    public class TimeSlot
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public int DurationInMinutes { get; set; }

        public DateTime GetStartDateTime() => Date.ToDateTime(StartTime);
        public DateTime GetEndDateTime() => Date.ToDateTime(StartTime).AddMinutes(DurationInMinutes);

        public override string ToString()
        {
            return $"{Date} {StartTime} ({DurationInMinutes} minutes)";
        }
    }
}
