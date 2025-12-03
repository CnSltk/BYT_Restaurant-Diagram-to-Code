namespace RestaurantTests;

[TestFixture]
public class CashierTests
{
    private string _testFilePath;
    
    [SetUp]
    public void Setup()
    {
        var extentField = typeof(Cashier).GetField("_cashierExtent");
        extentField?.SetValue(null, new List<Cashier>());
        
        _testFilePath = Path.Combine(Path.GetTempPath(), $"cashiers_test_{Guid.NewGuid()}.json");
    }
    
    [TearDown]
    public void Teardown()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
    }
    
    [Test]
    public void Constructor_NullFirstName()
    {
        Assert.Throws<ArgumentException>(() => new Cashier(1, null, "Ogus", 3000m, "Front"));
    }

    [Test]
    public void Constructor_EmptyLastName()
    {
        Assert.Throws<ArgumentException>(() => new Cashier(2, "Derya", "", 3000m, "Front"));
    }

    [Test]
    public void Constructor_NegativeSalary()
    {
        Assert.Throws<ArgumentException>(() => new Cashier(3, "Derya", "Ogus", -100m, "Front"));
    }

    

    [Test]
    public void ReceivePayment_InvalidAmount()
    {
        var cashier = new Cashier(4, "Derya", "Ogus", 3000m, "Front");
        
        Assert.Throws<ArgumentException>(() => cashier.ReceivePayment(0m, "Cash", "ORDER-123"));
        Assert.Throws<ArgumentException>(() => cashier.ReceivePayment(-10m, "Cash", "ORDER-123"));
    }

    [Test]
    public void ReceivePayment_NullPaymentMethod()
    {
        var cashier = new Cashier(5, "Derya", "Ogus", 3000m, "Front");
        
        Assert.Throws<ArgumentException>(() => cashier.ReceivePayment(50m, null, "ORDER-123"));
        Assert.Throws<ArgumentException>(() => cashier.ReceivePayment(50m, "", "ORDER-123"));
        Assert.Throws<ArgumentException>(() => cashier.ReceivePayment(50m, "   ", "ORDER-123"));
    }
    

    [Test]
    public void IssueRefund_InvalidAmount()
    {
        var cashier = new Cashier(6, "Derya", "Ogus", 3000m, "Front");
        
        Assert.Throws<ArgumentException>(() => cashier.IssueRefund(0m, "ORDER-123", "Reason", "AUTH-456"));
        Assert.Throws<ArgumentException>(() => cashier.IssueRefund(-10m, "ORDER-123", "Reason", "AUTH-456"));
    }

    [Test]
    public void Save_WithValidData()
    {
        new Cashier(7, "Derya", "Ogus", 3000m, "Front");
        new Cashier(8, "Can", "Saltik", 3200m, "Front");
        
        Cashier.Save(_testFilePath);
        
        Assert.That(File.Exists(_testFilePath), Is.True);
        var json = File.ReadAllText(_testFilePath);
        Assert.That(json, Does.Contain("Derya"));
        Assert.That(json, Does.Contain("Ogus"));
        Assert.That(json, Does.Contain("3000"));
        Assert.That(json, Does.Contain("Can"));
        Assert.That(json, Does.Contain("Saltik"));
    }

    [Test]
    public void Load_WithValidFile()
    {
        var testData = "[{\"FirstName\":\"Derya\",\"LastName\":\"Ogus\",\"Salary\":3100,\"Department\":\"Front\"}]";
        File.WriteAllText(_testFilePath, testData);
        
        bool result = Cashier.Load(_testFilePath);
        var extent = Cashier.GetExtent();
        
        Assert.That(result, Is.True);
        Assert.That(extent.Count, Is.EqualTo(1));
        Assert.That(extent[0].FirstName, Is.EqualTo("Derya"));
        Assert.That(extent[0].Salary, Is.EqualTo(3100m));
    }

    [Test]
    public void Load_WithNonExistentFile()
    {
        new Cashier(9, "Derya", "Ogus", 3000m, "Front");
        
        bool result = Cashier.Load("nonexistent.json");
        
        Assert.That(result, Is.False);
        Assert.That(Cashier.GetExtent().Count, Is.EqualTo(0));
    }

    [Test]
    public void Load_WithInvalidJson()
    {
        File.WriteAllText(_testFilePath, "invalid json content");
        
        bool result = Cashier.Load(_testFilePath);
        
        Assert.That(result, Is.False);
        Assert.That(Cashier.GetExtent().Count, Is.EqualTo(0));
    }
    
}