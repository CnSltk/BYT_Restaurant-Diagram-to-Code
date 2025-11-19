using Menu;
using Main.Classes.Employees;
using Main.Classes.Orders;

namespace RestaurantTests;


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




[TestFixture]
public class CustomerTests
{
    [Test]
    public void CustomerID_MustBeGreaterThanZero()
    {
        Assert.Throws<ArgumentException>(() =>
            new Customer(
                0,
                "Ibrahim",
                "Yesil",
                "453043988",
                "s30066@pjwstk.edu.pl"
            ));
    }

    [Test]
    public void PhoneNumber_MustContainDigitsOnly()
    {
        Assert.Throws<ArgumentException>(() =>
            new Customer(
                1,
                "Ibrahim",
                "Yesil",
                "45A03988",
                "s30066@pjwstk.edu.pl"
            ));
    }

    [Test]
    public void Email_MustContain_At_And_Dot()
    {
        Assert.Throws<ArgumentException>(() =>
            new Customer(
                1,
                "Ibrahim",
                "Yesil",
                "453043988",
                "invalidEmail" 
            ));
    }
}


[TestFixture]
public class DeliveryTests
{
    private Adress ValidAdress => new Adress
    {
        City = "Warsaw",
        StreetName = "Main Street",
        ZipCode = "00-001"
    };

    [Test]
    public void DeliveryID_MustBe_GreaterThanZero()
    {
        Assert.Throws<ArgumentException>(() =>
            new Delivery(
                -1,
                DeliveryMethod.Courier,
                ValidAdress,
                DateTime.Now,
                null,
                10m,
                DeliveryStatus.Scheduled
            ));
    }

    [Test]
    public void DeliveredAt_CannotBeEarlierThan_ScheduledAt()
    {
        var scheduled = DateTime.Now;
        var delivered = scheduled.AddHours(-1); 

        Assert.Throws<ArgumentException>(() =>
            new Delivery(
                1,
                DeliveryMethod.Courier,
                ValidAdress,
                scheduled,
                delivered,
                10m,
                DeliveryStatus.Scheduled
            ));
    }

    [Test]
    public void Address_CannotBeNull_OrIncomplete()
    {
        Assert.Throws<ArgumentException>(() =>
            new Delivery(
                1,
                DeliveryMethod.Courier,
                new Adress { City = "", StreetName = "A", ZipCode = "123" },
                DateTime.Now,
                null,
                5m,
                DeliveryStatus.OnRoute
            ));
    }
}


[TestFixture]
public class OrderTests
{
    [Test]
    public void OrderTime_CannotBeInFuture()
    {
        Assert.Throws<ArgumentException>(() =>
            new Order(
                DateTime.Now.AddHours(1), 
                true,
                OrderStatus.Preparing
            ));
    }

    [Test]
    public void Order_IsAddedToExtent_OnCreation()
    {
        var countBefore = Order.Extent.Count;

        var o = new Order(
            DateTime.Now.AddMinutes(-10),
            false,
            OrderStatus.Preparing
        );

        Assert.That(Order.Extent.Count, Is.EqualTo(countBefore + 1));
        Assert.That(Order.Extent.Contains(o));
    }

    [Test]
    public void OrderPrepDuration_IsCalculatedCorrectly()
    {
        var tenMinutesAgo = DateTime.Now.AddMinutes(-10);

        var o = new Order(tenMinutesAgo, false, OrderStatus.Prepared);

        Assert.That(o.OrderPrepDuration.TotalMinutes,
            Is.GreaterThanOrEqualTo(9).And.LessThanOrEqualTo(11));
    }
}



[TestFixture]
public class PaymentTests
{
    [Test]
    public void PaymentID_MustBeGreaterThanZero()
    {
        Assert.Throws<ArgumentException>(() =>
            new Payment(
                50m,
                DateTime.Now,
                0, 
                PaymentMethod.Cash,
                PaymentStatus.Pending,
                null
            ));
    }

    [Test]
    public void PaymentTime_CannotBeInFuture()
    {
        Assert.Throws<ArgumentException>(() =>
            new Payment(
                10m,
                DateTime.Now.AddHours(2), 
                1,
                PaymentMethod.Card,
                PaymentStatus.Pending,
                null
            ));
    }

    [Test]
    public void PaidAt_CannotBeEarlierThan_PaymentTime()
    {
        var paymentTime = DateTime.Now.AddMinutes(-5);
        var paidAt = paymentTime.AddMinutes(-10); 

        Assert.Throws<ArgumentException>(() =>
            new Payment(
                20m,
                paymentTime,
                1,
                PaymentMethod.Online,
                PaymentStatus.Completed,
                paidAt
            ));
    }
}


