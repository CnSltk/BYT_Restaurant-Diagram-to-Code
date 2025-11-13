using Main.Classes.Employees;

namespace RestaurantTests;

[TestFixture]
public class ClassConstructions
{
    [TestFixture]
    public class EmployeesStaff
    {
        // A testable derived class because Staff is abstract
        public class TestableStaff : Staff
        {
            public TestableStaff(string firstName, string lastName, decimal salary, string department)
                : base(firstName, lastName, salary, department)
            {
            }

            public override void hireStaff()
            {
            }

            public override void fireStaff()
            {
            }
        }

        [Test]
        public void ConstructorValidValues()
        {
            var staff = new TestableStaff("Derya", "Ogus", 50000, "IT");

            Assert.That(staff, Is.Not.Null);
            Assert.That(staff.FirstName, Is.EqualTo("Derya"));
            Assert.That(staff.LastName, Is.EqualTo("Ogus"));
            Assert.That(staff.Salary, Is.EqualTo(50000));
            Assert.That(staff.Department, Is.EqualTo("IT"));
        }

        [Test]
        public void ConstructorWithNullFirstName()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new TestableStaff(null, "Ogus", 50000, "IT"));
            Assert.That(ex.Message, Is.EqualTo("First name cannot be empty"));
        }
        [Test]
        public void ConstructorWithEmptyFirstName()
        {
            var ex = Assert.Throws<ArgumentException>(() => 
                new TestableStaff("", "Ogus", 50000, "IT"));
            Assert.That(ex.Message, Is.EqualTo("First name cannot be empty"));
        }
    }
}