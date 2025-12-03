using Main.Classes.Employees;

namespace RestaurantTests;

[TestFixture]
public class ManagerTests
{
    private string _testFilePath;
    
    [SetUp]
    public void Setup()
    {
        var extentField = typeof(Manager).GetField("_managerExtent");
        extentField?.SetValue(null, new List<Manager>());
        
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
        new Manager(1,"Arda", "Seydol", 7000m, "Management", ManagerLevels.Senior);
        
        Manager.Save(_testFilePath);
        
        var extentField = typeof(Manager).GetField("_managerExtent");
        extentField?.SetValue(null, new List<Manager>());
        
        bool result = Manager.Load(_testFilePath);
        var extent = Manager.GetExtent();
        
        Assert.That(result, Is.True);
        Assert.That(extent.Count, Is.EqualTo(1));
    }
    

    [Test]
    public void HireStaff()
    {
        var manager = new Manager(2,"Derya", "Ogus", 6000m, "Management", ManagerLevels.Mid);
        
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        
        manager.hireStaff();
        
        Assert.That(consoleOutput.ToString(), Does.Contain("Manager Ogus is hiring staff."));
    }

    [Test]
    public void FireStaff()
    {
        var manager = new Manager(3,"Derya", "Ogus", 6000m, "Management", ManagerLevels.Junior);
        
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        
        manager.fireStaff();
        
        Assert.That(consoleOutput.ToString(), Does.Contain("Manager Ogus is firing staff."));
    }

    [Test]
    public void ManageEmployee()
    {
        var manager = new Manager(4,"Derya", "Ogus", 6000m, "Management", ManagerLevels.Senior);
        
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        
        manager.ManageEmployee();
        
        Assert.That(consoleOutput.ToString(), Does.Contain("Manager Ogus is managing employees."));
    }

    [Test]
    public void ChangeStaffShift()
    {
        var manager = new Manager(5,"Derya", "Ogus", 6000m, "Management", ManagerLevels.Mid);
        
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        
        manager.ChangeStaffShift();
        
        Assert.That(consoleOutput.ToString(), Does.Contain("Manager Ogus is changing staff shift."));
    }

   
}