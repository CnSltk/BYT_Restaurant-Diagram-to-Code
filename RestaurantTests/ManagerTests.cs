using Main.Classes.Employees;

namespace RestaurantTests;

[TestFixture]
public class ManagerTests
{
    private string _testFilePath;
    
    [SetUp]
    public void Setup()
    {
        var extentField = typeof(Staff).GetField("_managerExtent");
        extentField?.SetValue(null, new List<Staff>());
        
        _testFilePath = Path.Combine(Path.GetTempPath(), $"managers_test_{Guid.NewGuid()}.json");
    }
    
    [TearDown]
    public void Teardown()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
    }
    
    [Test]
    public void SaveAndLoad()
    {
        new Staff(1,"Arda", "Seydol", 7000m, "Management", StaffType.Manager);
        
        Staff.Save(_testFilePath);
        
        var extentField = typeof(Staff).GetField("_managerExtent");
        extentField?.SetValue(null, new List<Staff>());
        
        bool result = Staff.Load(_testFilePath);
        var extent = Staff.GetExtent();
        
        Assert.That(result, Is.True);
        Assert.That(extent.Count, Is.EqualTo(1));
    }
    

    [Test]
    public void HireStaff()
    {
        var manager = new Staff(2,"Derya", "Ogus", 6000m, "Management", StaffType.Manager);
        
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        
        manager.HireStaff();
        
        Assert.That(consoleOutput.ToString(), Does.Contain("Manager Ogus is hiring staff."));
    }

    [Test]
    public void FireStaff()
    {
        var manager = new Staff(3,"Derya", "Ogus", 6000m, "Management", StaffType.Manager);
        
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        
        manager.FireStaff();
        
        Assert.That(consoleOutput.ToString(), Does.Contain("Manager Ogus is firing staff."));
    }

    [Test]
    public void ManageEmployee()
    {
        var manager = new Staff(4,"Derya", "Ogus", 6000m, "Management", StaffType.Manager);
        
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        
        manager.ManageEmployees();
        
        Assert.That(consoleOutput.ToString(), Does.Contain("Manager Ogus is managing employees."));
    }

    [Test]
    public void ChangeStaffShift()
    {
        var manager = new Staff(5,"Derya", "Ogus", 6000m, "Management", StaffType.Manager);
        
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        
        manager.ChangeStaffShift();
        
        Assert.That(consoleOutput.ToString(), Does.Contain("Manager Ogus is changing staff shift."));
    }

   
}