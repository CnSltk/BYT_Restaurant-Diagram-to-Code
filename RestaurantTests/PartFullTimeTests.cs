using Main.Classes.Employees;

namespace RestaurantTests;

[TestFixture]
public class PartFullTimeTests
{
    //part time
    [Test]
    public void Constructor_WithNonPositiveHours_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new PartTime(1,"Can", "Saltik", "Kitchen", 0, 10m, 100,StaffType.Manager));

        Assert.Throws<ArgumentException>(() =>
            new PartTime(2,"Can", "Saltik", "Kitchen", -5, 10m,100,StaffType.Manager));
    }

    [Test]
    public void Constructor_WithNonPositiveHourlyRate_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new PartTime(3,"Arda", "Yesil", "Service", 10, 0m,100,StaffType.Manager));

        Assert.Throws<ArgumentException>(() =>
            new PartTime(4,"Arda", "Yesil", "Service", 10, -1m, 100,StaffType.Manager));
    }

    [Test]
    public void WeeklySalary_ReflectsChanges_WhenBothValuesChange()
    {
        var pt = new PartTime(5,"Arda", "Yesil", "Service", 10, 20m, 100,StaffType.Manager);
        Assert.That(pt.WeeklySalary, Is.EqualTo(200m));

        pt.Hours = 15;
        pt.HourlyRate = 25m;

        Assert.That(pt.WeeklySalary, Is.EqualTo(375m));
        Assert.That(pt.Salary, Is.EqualTo(375m));
    }
    
    //full time
    [Test]
    public void Constructor_AssignsPropertiesCorrectly()
    {
        var ft = new FullTime(6,"Melisa", "Arslan", 4000m, "Service", Shift.Evening, StaffType.Manager);

        Assert.That(ft.FirstName, Is.EqualTo("Melisa"));
        Assert.That(ft.LastName, Is.EqualTo("Arslan"));
        Assert.That(ft.Salary, Is.EqualTo(4000m));
        Assert.That(ft.Department, Is.EqualTo("Service"));
        Assert.That(ft.Shift, Is.EqualTo(Shift.Evening));
    }

    [Test]
    public void ShiftChange_CanSetSameShiftWithoutError()
    {
        var ft = new FullTime(7,"Derya", "Ogus", 5000m, "Cashier", Shift.Morning, StaffType.Manager);

        // Changing to the same shift should be harmless
        ft.ShiftChange(Shift.Morning);

        Assert.That(ft.Shift, Is.EqualTo(Shift.Morning));
    }
    
    [Test]
    public void WeeklySalary_ComputedCorrectly_AndSynced()
    {
        var pt = new PartTime(8,"Can", "Saltik", "Chef", hours: 20, hourlyRate: 15m, 100, StaffType.Manager);

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
        var pt = new PartTime(9,"Ibrahim", "Yesil", "Waiter", 10, 10m, 100, StaffType.Manager);

        Assert.Throws<ArgumentException>(() => pt.Hours = 0);
        Assert.Throws<ArgumentException>(() => pt.Hours = -5);
    }

    [Test]
    public void Setting_NonPositiveHourlyRate_Throws()
    {
        var pt = new PartTime(0,"Arda", "Seydol", "Manager", 10, 10m, 100, StaffType.Manager);

        Assert.Throws<ArgumentException>(() => pt.HourlyRate = 0m);
        Assert.Throws<ArgumentException>(() => pt.HourlyRate = -1m);
    }
}