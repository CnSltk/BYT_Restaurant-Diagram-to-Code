using Main.Classes.Employees;

namespace RestaurantTests;
[TestFixture]
public class CanTests
{
[Test]
public void WeeklySalary_Computed_AndBaseSalaryInSync()
    {
        var pt = new PartTime("Can", "Saltik", "Chef", hours: 20, hourlyRate: 15m);
        Assert.That(pt.WeeklySalary, Is.EqualTo(300m)); // 20 * 15
        Assert.That(pt.Salary, Is.EqualTo(300m));       // synced
        pt.Hours = 25;
        Assert.That(pt.WeeklySalary, Is.EqualTo(375m));
        Assert.That(pt.Salary, Is.EqualTo(375m));
        pt.HourlyRate = 12m;
        Assert.That(pt.WeeklySalary, Is.EqualTo(300m)); // 25 * 12
        Assert.That(pt.Salary, Is.EqualTo(300m));
    }

[Test]
public void Setting_NonPositiveHours_Throws()
    {
        var pt = new PartTime("Ibrahim", "Yesil", "Waiter", hours: 10, hourlyRate: 10m);
        Assert.Throws<ArgumentException>(() => pt.Hours = 0);
        Assert.Throws<ArgumentException>(() => pt.Hours = -5);
    }

[Test]
public void Setting_NonPositiveHourlyRate_Throws()
    {
        var pt = new PartTime("Arda", "Seydol", "Manager", hours: 10, hourlyRate: 10m);
        Assert.Throws<ArgumentException>(() => pt.HourlyRate = 0m);
        Assert.Throws<ArgumentException>(() => pt.HourlyRate = -1m);
    }
[Test]
public void ShiftChange_UpdatesShift()
    {  
        var ft = new FullTime("Derya", "Ogus", salary: 5000m, department: "Cashier", shift: Shift.Morning);
        Assert.That(ft.Shift, Is.EqualTo(Shift.Morning));
        ft.ShiftChange(Shift.Night);
        Assert.That(ft.Shift, Is.EqualTo(Shift.Night));
        ft.ShiftChange(Shift.Evening);
        Assert.That(ft.Shift, Is.EqualTo(Shift.Evening));
    }
}
