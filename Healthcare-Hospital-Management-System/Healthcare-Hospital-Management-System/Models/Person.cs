namespace HealthcareHospitalManagementSystem.Models
{
    public abstract class Person
    {
        private string _name = string.Empty;
        private int _age;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be null or empty", nameof(value));
                _name = value;
            }
        }

        public int Age
        {
            get => _age;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Age cannot be negative", nameof(value));
                _age = value;
            }
        }

        public abstract string DisplayInfo();

        public virtual string Work()
        {
            return $"{Name} is working.";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Person other)
            {
                return Name == other.Name && Age == other.Age;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Age);
        }
    }
}
