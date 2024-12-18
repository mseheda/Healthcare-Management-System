using Healthcare_Hospital_Management_System.Models;

namespace HealthcareHospitalManagementSystem.Models
{
    public class Doctor : Person
    {
        public required string Specialization { get; set; } 
        public required string Department { get; set; }
        public List<TimeSlot> AvailableTimeSlots { get; set; } = new List<TimeSlot>();

        public override string DisplayInfo()
        {
            return $"Doctor {Name}, Age: {Age}, Specialization: {Specialization}, Department: {Department}";
        }

        public override string Work()
        {
            return $"Doctor {Name} is working with patients.";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Doctor other)
            {
                return base.Equals(other) &&
                       this.Specialization == other.Specialization &&
                       this.Department == other.Department;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Specialization, Department);
        }
    }
}