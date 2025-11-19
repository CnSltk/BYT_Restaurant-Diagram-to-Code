using Menu;
using Main.Classes.Employees;
using Main.Classes.Orders;

namespace RestaurantTests;

/* -------------------------------------------------------------
   MENU ITEM TESTS
--------------------------------------------------------------*/
[TestFixture]
public class MenuItemTests
{
    public class TestableMenuItem : MenuItems
    {
        public TestableMenuItem(
            string name,
            decimal price,
            bool isAvailable,
            string? description = null
        ) : base(name, price, isAvailable, description)
        {
        }
    }

    [Test]
    public void Constructor_ValidValues_AssignedCorrectly()
    {
        var item = new TestableMenuItem("Pizza", 20, true, "Cheesy");

        Assert.That(item.Name, Is.EqualTo("Pizza"));
        Assert.That(item.Price, Is.EqualTo(20));
        Assert.That(item.IsAvailable, Is.True);
        Assert.That(item.Description, Is.EqualTo("Cheesy"));
    }

    [Test]
    public void Constructor_EmptyName_ThrowsException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new TestableMenuItem("", 10, true)
        );

        Assert.That(ex.Message, Is.EqualTo("Name cannot be empty"));
    }

    [Test]
    public void UpdateMenuItem_UpdatesAllFields()
    {
        var item = new TestableMenuItem("Burger", 18, true);

        item.UpdateMenuItem(
            name: "Veggie Burger",
            price: 20,
            isAvailable: false,
            description: "New!"
        );

        Assert.That(item.Name, Is.EqualTo("Veggie Burger"));
        Assert.That(item.Price, Is.EqualTo(20));
        Assert.That(item.IsAvailable, Is.False);
        Assert.That(item.Description, Is.EqualTo("New!"));
    }
}



/* -------------------------------------------------------------
   PART-TIME EMPLOYEE TESTS
--------------------------------------------------------------*/
[TestFixture]
public class EmployeePartTimeTests
{
    [Test]
    public void WeeklySalary_ComputedCorrectly_AndSynced()
    {
        var pt = new PartTime("Can", "Saltik", "Chef", hours: 20, hourlyRate: 15m);

        Assert.That(pt.WeeklySalary, Is.EqualTo(300m));
        Assert.That(pt.Salary, Is.EqualTo(300m));

        pt.Hours = 25;
        Assert.That(pt.WeeklySalary, Is.EqualTo(375m));
        Assert.That(pt.Salary, Is.EqualTo(375m));

        pt.HourlyRate = 12m;
        Assert.That(pt.WeeklySalary, Is.EqualTo(300m));
        Assert.That(pt.Salary, Is.EqualTo(300m));
    }

    [Test]
    public void Setting_NonPositiveHours_Throws()
    {
        var pt = new PartTime("Ibrahim", "Yesil", "Waiter", 10, 10m);

        Assert.Throws<ArgumentException>(() => pt.Hours = 0);
        Assert.Throws<ArgumentException>(() => pt.Hours = -5);
    }

    [Test]
    public void Setting_NonPositiveHourlyRate_Throws()
    {
        var pt = new PartTime("Arda", "Seydol", "Manager", 10, 10m);

        Assert.Throws<ArgumentException>(() => pt.HourlyRate = 0m);
        Assert.Throws<ArgumentException>(() => pt.HourlyRate = -1m);
    }
}



/* -------------------------------------------------------------
   FULL-TIME EMPLOYEE TESTS
--------------------------------------------------------------*/
[TestFixture]
public class EmployeeFullTimeTests
{
    [Test]
    public void ShiftChange_UpdatesShiftCorrectly()
    {
        var ft = new FullTime("Derya", "Ogus", 5000m, "Cashier", Shift.Morning);

        Assert.That(ft.Shift, Is.EqualTo(Shift.Morning));

        ft.ShiftChange(Shift.Night);
        Assert.That(ft.Shift, Is.EqualTo(Shift.Night));

        ft.ShiftChange(Shift.Evening);
        Assert.That(ft.Shift, Is.EqualTo(Shift.Evening));
    }
}



/* -------------------------------------------------------------
   STAFF BASE CLASS TESTS
--------------------------------------------------------------*/
[TestFixture]
public class EmployeeStaffTests
{
    public class TestableStaff : Staff
    {
        public TestableStaff(string firstName, string lastName, decimal salary, string department)
            : base(firstName, lastName, salary, department)
        {
        }

        public override void hireStaff() { }
        public override void fireStaff() { }
    }

    [Test]
    public void Constructor_ValidValues_AssignedCorrectly()
    {
        var staff = new TestableStaff("Derya", "Ogus", 50000, "IT");

        Assert.That(staff.FirstName, Is.EqualTo("Derya"));
        Assert.That(staff.LastName, Is.EqualTo("Ogus"));
        Assert.That(staff.Salary, Is.EqualTo(50000));
        Assert.That(staff.Department, Is.EqualTo("IT"));
    }

    [Test]
    public void Constructor_NullFirstName_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new TestableStaff(null, "Ogus", 50000, "IT")
        );

        Assert.That(ex.Message, Is.EqualTo("First name cannot be empty"));
    }

    [Test]
    public void Constructor_EmptyFirstName_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new TestableStaff("", "Ogus", 50000, "IT")
        );

        Assert.That(ex.Message, Is.EqualTo("First name cannot be empty"));
    }
}



/* -------------------------------------------------------------
   CUSTOMER CLASS TESTS
--------------------------------------------------------------*/
[TestFixture]
public class CustomerTests
{
    [Test]
    public void Constructor_ValidValues_AssignedCorrectly()
    {
        var c = new Customer(1, "Ibrahim", "Yesil", "453043988", "s30066@pjwstk.edu.pl");

        Assert.That(c.Name, Is.EqualTo("Ibrahim"));
        Assert.That(c.Surname, Is.EqualTo("Yesil"));
        Assert.That(c.PhoneNumber, Is.EqualTo("453043988"));
        Assert.That(c.Email, Is.EqualTo("s30066@pjwstk.edu.pl"));
    }

    [Test]
    public void Constructor_InvalidId_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new Customer(-3, "Ibrahim", "Yesil", null, null)
        );

        Assert.That(ex.Message, Is.EqualTo("CustomerId cannot be less than 1."));
    }

    [Test]
    public void Constructor_EmptySurname_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new Customer(2, "Ibrahim", "", null, null)
        );

        Assert.That(ex.Message, Is.EqualTo("Surname cannot be empty"));
    }
}
