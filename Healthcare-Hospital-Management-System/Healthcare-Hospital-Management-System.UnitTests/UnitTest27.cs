using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HealthcareHospitalManagementSystem.Models; // For Doctor and Person
using Healthcare_Hospital_Management_System.Models; // For TimeSlot

namespace Healthcare_Hospital_Management_System.UnitTests
{
    [TestClass]
    public class EntitiesTests
    {
        [TestMethod]
        public void Person_And_Doctor_TestAll()
        {
            var doc = new Doctor();
            doc.Name = "Alice";
            Assert.AreEqual("Alice", doc.Name);

            doc.Age = 35;
            Assert.AreEqual(35, doc.Age);

            // Test Age negative scenario (exception)
            Assert.ThrowsException<ArgumentException>(() => doc.Age = -1);

            // Test Doctor-specific properties
            doc.Specialization = "Cardiology";
            Assert.AreEqual("Cardiology", doc.Specialization);

            doc.Department = "Cardio Dept";
            Assert.AreEqual("Cardio Dept", doc.Department);

            // Test AvailableTimeSlots initialization and usage
            Assert.IsNotNull(doc.AvailableTimeSlots);
            Assert.AreEqual(0, doc.AvailableTimeSlots.Count);
            doc.AvailableTimeSlots.Add(new TimeSlot
            {
                Date = DateOnly.FromDateTime(DateTime.Today),
                StartTime = TimeOnly.FromDateTime(DateTime.Now),
                DurationInMinutes = 30
            });
            Assert.AreEqual(1, doc.AvailableTimeSlots.Count);

            // Test DisplayInfo() override
            var displayInfo = doc.DisplayInfo();
            Assert.IsTrue(displayInfo.Contains("Doctor Alice"));
            Assert.IsTrue(displayInfo.Contains("Age: 35"));
            Assert.IsTrue(displayInfo.Contains("Specialization: Cardiology"));
            Assert.IsTrue(displayInfo.Contains("Department: Cardio Dept"));

            // Test Work() override
            var workResult = doc.Work();
            Assert.AreEqual("Doctor Alice is working with patients.", workResult);

            var tempPerson = new TestPerson { Name = "Bob", Age = 40 };
            Assert.AreEqual("Bob is working.", tempPerson.Work()); // Covers Person's virtual Work() method

            // Test Equals and GetHashCode for Person and Doctor
            // Equals with same values
            var doc2 = new Doctor
            {
                Name = "Alice",
                Age = 35,
                Specialization = "Cardiology",
                Department = "Cardio Dept"
            };
            Assert.IsTrue(doc.Equals(doc2));
            Assert.AreEqual(doc.GetHashCode(), doc2.GetHashCode());

            // Equals with different Name
            var doc3 = new Doctor
            {
                Name = "AliceX",
                Age = 35,
                Specialization = "Cardiology",
                Department = "Cardio Dept"
            };
            Assert.IsFalse(doc.Equals(doc3));

            // Equals with different Age
            var doc4 = new Doctor
            {
                Name = "Alice",
                Age = 36,
                Specialization = "Cardiology",
                Department = "Cardio Dept"
            };
            Assert.IsFalse(doc.Equals(doc4));

            // Equals with different Specialization
            var doc5 = new Doctor
            {
                Name = "Alice",
                Age = 35,
                Specialization = "Neurology",
                Department = "Cardio Dept"
            };
            Assert.IsFalse(doc.Equals(doc5));

            // Equals with different Department
            var doc6 = new Doctor
            {
                Name = "Alice",
                Age = 35,
                Specialization = "Cardiology",
                Department = "Some Other Dept"
            };
            Assert.IsFalse(doc.Equals(doc6));

            Assert.IsFalse(doc.Equals(null));

            Assert.IsFalse(doc.Equals("Not a doctor"));

            var person1 = new TestPerson { Name = "Charlie", Age = 50 };
            var person2 = new TestPerson { Name = "Charlie", Age = 50 };
            Assert.IsTrue(person1.Equals(person2));
            Assert.AreEqual(person1.GetHashCode(), person2.GetHashCode());

            var person3 = new TestPerson { Name = "Charlie", Age = 51 };
            Assert.IsFalse(person1.Equals(person3));

            var person4 = new TestPerson { Name = "Chuck", Age = 50 };
            Assert.IsFalse(person1.Equals(person4));

            Assert.IsFalse(person1.Equals("Not a Person"));

            Assert.IsTrue(doc.Equals(doc));
        }

        [TestMethod]
        public void TimeSlot_TestAll()
        {
            // Arrange
            var slot = new TimeSlot
            {
                Date = DateOnly.FromDateTime(new DateTime(2024, 12, 8)),
                StartTime = new TimeOnly(14, 30),
                DurationInMinutes = 45
            };

            // Act
            var start = slot.GetStartDateTime();
            var end = slot.GetEndDateTime();
            var toStringResult = slot.ToString();

            // Assert
            Assert.AreEqual(new DateTime(2024, 12, 8, 14, 30, 0), start, "StartDateTime does not match.");
            Assert.AreEqual(new DateTime(2024, 12, 8, 15, 15, 0), end, "EndDateTime does not match.");

            Console.WriteLine($"ToString Result: {toStringResult}");

            Assert.AreEqual(
                "08.12.2024 14:30 (45 minutes)", // Match system format
                toStringResult,
                "ToString() result does not match the expected format."
            );
        }

        private class TestPerson : Person
        {
            public override string DisplayInfo()
            {
                return $"TestPerson {Name}, Age: {Age}";
            }
        }
    }
}
