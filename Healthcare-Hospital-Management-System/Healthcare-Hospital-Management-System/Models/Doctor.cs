namespace HealthcareHospitalManagementSystem.Models
{
    public class Doctor : Person
    {
        public string Specialization { get; set; }
        public string Department { get; set; }
        public List<DateTime> AvailableDates { get; set; } = new List<DateTime>();

        public override string DisplayInfo()
        {
            return $"Doctor {Name}, Age: {Age}, Specialization: {Specialization}, Department: {Department}";
        }

        public override string Work()
        {
            return $"Doctor {Name} is working with patients.";
        }

        public override bool Equals(object obj)
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