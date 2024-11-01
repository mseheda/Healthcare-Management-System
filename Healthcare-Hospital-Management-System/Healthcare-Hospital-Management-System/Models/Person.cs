namespace HealthcareHospitalManagementSystem.Models
{
    public abstract class Person
    {
        private string _name;
        private int _age;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Age
        {
            get { return _age; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Age cannot be negative");
                _age = value;
            }
        }

        public abstract string DisplayInfo();

        public virtual string Work()
        {
            return $"{Name} is working.";
        }

        public override bool Equals(object obj)
        {
            if (obj is Person other)
            {
                return this.Name == other.Name && this.Age == other.Age;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Age);
        }
    }
}