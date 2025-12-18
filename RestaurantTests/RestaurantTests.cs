using Menu;
using Main.Classes.Employees;
using Main.Classes.Orders;



namespace RestaurantTests;


[TestFixture]
public class MenuItemTests
{
    [SetUp]
    public void Setup()
    {
        MenuItems.ClearExtentForTests();
    }

    [Test]
    public void CreateFood_ValidValues_AssignedCorrectly()
    {
        var item = MenuItems.CreateFood(
            name: "Pizza",
            price: 20m,
            isAvailable: true,
            spiceLevel: true,
            isVegetarian: false,
            calories: 800,
            prepTimeMin: 15,
            category: FoodCategory.MainCourse,
            description: "Cheesy"
        );

        Assert.That(item.Name, Is.EqualTo("Pizza"));
        Assert.That(item.Price, Is.EqualTo(20m));
        Assert.That(item.IsAvailable, Is.True);
        Assert.That(item.Description, Is.EqualTo("Cheesy"));
        Assert.That(item.ItemType, Is.EqualTo(MenuItemType.Food));
        Assert.That(item.Calories, Is.EqualTo(800));
    }

    [Test]
    public void CreateFood_WithBeverageAttribute_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            var item = MenuItems.CreateFood(
                "Soup", 10m, true,
                false, true, 200, 10,
                FoodCategory.Starter
            );

            item.UpdateBeverageDetails(volumeMl: 500);
        });
    }

    [Test]
    public void Name_Empty_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            MenuItems.CreateFood(
                "", 10m, true,
                false, true, 300, 10,
                FoodCategory.Starter
            ));
    }

    [Test]
    public void UpdateBase_UpdatesCommonFields()
    {
        var item = MenuItems.CreateFood(
            "Burger", 18m, true,
            false, false, 700, 15,
            FoodCategory.MainCourse
        );

        item.UpdateBase(
            name: "Veggie Burger",
            price: 20m,
            isAvailable: false,
            description: "New!"
        );

        Assert.That(item.Name, Is.EqualTo("Veggie Burger"));
        Assert.That(item.Price, Is.EqualTo(20m));
        Assert.That(item.IsAvailable, Is.False);
        Assert.That(item.Description, Is.EqualTo("New!"));
    }
}


/* -------------------------------------------------------------
   FOOD TESTS
--------------------------------------------------------------*/
[TestFixture]
public class BeverageMenuItemTests
{
    [SetUp]
    public void Setup()
    {
        MenuItems.ClearExtentForTests();
    }

    [Test]
    public void CreateBeverage_ValidValues_AssignedCorrectly()
    {
        var drink = MenuItems.CreateBeverage(
            name: "Cola",
            price: 8m,
            isAvailable: true,
            volumeMl: 500,
            beverageCategory: BeverageCategory.SoftBeverage,
            description: "Cold"
        );

        Assert.That(drink.Name, Is.EqualTo("Cola"));
        Assert.That(drink.Price, Is.EqualTo(8m));
        Assert.That(drink.VolumeMl, Is.EqualTo(500));
        Assert.That(drink.BeverageCategory, Is.EqualTo(BeverageCategory.SoftBeverage));
        Assert.That(drink.ItemType, Is.EqualTo(MenuItemType.Beverage));
    }

    [Test]
    public void Beverage_InvalidVolume_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            MenuItems.CreateBeverage(
                "Water",
                5m,
                true,
                750,
                BeverageCategory.SoftBeverage
            ));
    }
}

/* -------------------------------------------------------------
   INGREDIENT TEST
--------------------------------------------------------------*/
[TestFixture]
public class IngredientTests
{
    [Test]
    public void Ingredient_ValidValues_AssignedCorrectly()
    {
        var ingredient = new Ingredient(
            ingredientId: 1,
            name: "Tomato",
            unit: Unit.Gram
        );

        Assert.That(ingredient.Name, Is.EqualTo("Tomato"));
        Assert.That(ingredient.Unit, Is.EqualTo(Unit.Gram));
    }
    
    
}

[TestFixture]
public class EmployeeStaffTests
{
    public class TestableStaff : Staff
    {
        public TestableStaff(int staffId, string firstName, string lastName, decimal salary, string department)
            : base(staffId, firstName, lastName, salary, department)
        {
        }

        public override void hireStaff() { }
        public override void fireStaff() { }
    }

    [Test]
    public void Constructor_ValidValues_AssignedCorrectly()
    {
        var staff = new TestableStaff(1,"Derya", "Ogus", 50000, "IT");

        Assert.That(staff.FirstName, Is.EqualTo("Derya"));
        Assert.That(staff.LastName, Is.EqualTo("Ogus"));
        Assert.That(staff.Salary, Is.EqualTo(50000));
        Assert.That(staff.Department, Is.EqualTo("IT"));
    }

    [Test]
    public void Constructor_NullFirstName_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new TestableStaff(1,null, "Ogus", 50000, "IT")
        );

        Assert.That(ex.Message, Is.EqualTo("First name cannot be empty"));
    }

    [Test]
    public void Constructor_EmptyFirstName_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new TestableStaff(1,"", "Ogus", 50000, "IT")
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

    private Customer DummyCustomer =>
        new Customer(999, "Test", "Tester", "123456", "test@test.com");

    private Order DummyOrder =>
        new Order(DateTime.Now.AddMinutes(-5), false, OrderStatus.Preparing, DummyCustomer);

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
                DeliveryStatus.Scheduled,
                DummyOrder   
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
                DeliveryStatus.Scheduled,
                DummyOrder  
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
                DeliveryStatus.OnRoute,
                DummyOrder  
            ));
    }
}




[TestFixture]
public class OrderTests
{
    private Customer DummyCustomer => 
        new Customer(1, "Test", "User", "123456", "test@test.com");

    [Test]
    public void OrderTime_CannotBeInFuture()
    {
        Assert.Throws<ArgumentException>(() =>
            new Order(
                DateTime.Now.AddHours(1),
                true,
                OrderStatus.Preparing,
                DummyCustomer
            ));
    }

    [Test]
    public void Order_IsAddedToExtent_OnCreation()
    {
        var countBefore = Order.Extent.Count;

        var o = new Order(
            DateTime.Now.AddMinutes(-10),
            false,
            OrderStatus.Preparing,
            DummyCustomer
        );

        Assert.That(Order.Extent.Count, Is.EqualTo(countBefore + 1));
        Assert.That(Order.Extent.Contains(o));
    }

    [Test]
    public void OrderPrepDuration_IsCalculatedCorrectly()
    {
        var tenMinutesAgo = DateTime.Now.AddMinutes(-10);

        var o = new Order(
            tenMinutesAgo,
            false,
            OrderStatus.Prepared,
            DummyCustomer
        );

        Assert.That(o.OrderPrepDuration.TotalMinutes,
            Is.GreaterThanOrEqualTo(9).And.LessThanOrEqualTo(11));
    }
}




[TestFixture]
public class PaymentTests
{
    private Order DummyOrder =>
        new Order(DateTime.Now.AddMinutes(-5), false, OrderStatus.Preparing,
            new Customer(2, "Pay", "Tester", "123456", "pay@test.com"));

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
                null,
                DummyOrder
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
                null,
                DummyOrder
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
                paidAt,
                DummyOrder
            ));
    }
}


[TestFixture]
public class ChefTests
{
    private string _testFilePath;
    
    [SetUp]
    public void Setup()
    {
        var extentField = typeof(Chef).GetField("_chefExtent");
        extentField?.SetValue(null, new List<Chef>());
        
        _testFilePath = Path.Combine(Path.GetTempPath(), $"chefs_test_{Guid.NewGuid()}.json");
    }
    
    [TearDown]
    public void Teardown()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
    }

    [Test]
    public void Constructor_InvalidFirstName()
    {
        Assert.Throws<ArgumentException>(() => new Chef(1,"", "Ramsay", 5000m, "Kitchen"));
    }

    [Test]
    public void SaveAndLoad()
    {
        new Chef(2,"Marco", "Pierre", 5500m, "Kitchen");
        
        Chef.Save(_testFilePath);
        var extentField = typeof(Chef).GetField("_chefExtent");
        extentField?.SetValue(null, new List<Chef>());
        
        bool result = Chef.Load(_testFilePath);
        var extent = Chef.GetExtent();
        
        Assert.That(result, Is.True);
        Assert.That(extent.Count, Is.EqualTo(1));
        Assert.That(extent[0].FirstName, Is.EqualTo("Marco"));
    }

    [Test]
    public void HireStaff()
    {
        var chef = new Chef(3,"Gordon", "Ramsay", 5000m, "Kitchen");
        
        Assert.Throws<NotImplementedException>(() => chef.hireStaff());
    }

    [Test]
    public void FireStaff()
    {
        var chef = new Chef(4,"Gordon", "Ramsay", 5000m, "Kitchen");
        
        Assert.Throws<NotImplementedException>(() => chef.fireStaff());
    }
}

[TestFixture]
public class HeadChefTests
{
    private string _testFilePath;
    
    [SetUp]
    public void Setup()
    {
        var chefExtentField = typeof(Chef).GetField("_chefExtent");
        chefExtentField?.SetValue(null, new List<Chef>());
        
        var headChefExtentField = typeof(HeadChef).GetField("_headChefExtent", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        headChefExtentField?.SetValue(null, new List<HeadChef>());
        
        _testFilePath = Path.Combine(Path.GetTempPath(), $"headchefs_test_{Guid.NewGuid()}.json");
    }
    
    [TearDown]
    public void Teardown()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
    }
    

    [Test]
    public void ManageInventory()
    {
        var headChef = new HeadChef(5,"Ibrahim", "Yesil", 8000m, "Kitchen", SignatureDish.FishTaco);
        
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        
        headChef.ManageInventory();
        
        Assert.That(consoleOutput.ToString(), Does.Contain("Head Chef Yesil is managing inventory."));
    }

    [Test]
    public void SaveAndLoad()
    {
        new HeadChef(6,"Arda", "Seydol", 9000m, "Kitchen", SignatureDish.Stew);
        
        HeadChef.Save(_testFilePath);
        
        var headChefExtentField = typeof(HeadChef).GetField("_headChefExtent");
        headChefExtentField?.SetValue(null, new List<HeadChef>());
        
        bool result = HeadChef.Load(_testFilePath);
        var extent = HeadChef.GetExtent();
        
        Assert.That(result, Is.True);
        Assert.That(extent.Count, Is.EqualTo(1));
        Assert.That(extent[0].LastName, Is.EqualTo("Seydol"));
        Assert.That(extent[0].Dish, Is.EqualTo(SignatureDish.Stew));
    }
}

[TestFixture]
public class WaiterTests
{
    private string _testFilePath;
    
    [SetUp]
    public void Setup()
    {
        var extentField = typeof(Waiter).GetField("_waiterExtent");
        extentField?.SetValue(null, new List<Waiter>());
        
        _testFilePath = Path.Combine(Path.GetTempPath(), $"waiters_test_{Guid.NewGuid()}.json");
    }
    
    [TearDown]
    public void Teardown()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
    }
    

    [Test]
    public void Constructor_NegativeTables()
    {
        Assert.Throws<ArgumentException>(() => new Waiter(1,"Rachel", "Green", 2500m, "Front", -1));
    }

    [Test]
    public void Tables_Setter_WithNegativeValue()
    {
        var waiter = new Waiter(1,"Rachel", "Green", 2500m, "Front", 3);
        
        Assert.Throws<ArgumentException>(() => waiter.Tables = -5);
    }

    

    [Test]
    public void SaveAndLoad()
    {
        new Waiter(1,"Monica", "Geller", 2600m, "Front", 4);
        
        Waiter.Save(_testFilePath);
        
        var extentField = typeof(Waiter).GetField("_waiterExtent");
        extentField?.SetValue(null, new List<Waiter>());
        
        bool result = Waiter.Load(_testFilePath);
        var extent = Waiter.GetExtent();
        
        Assert.That(result, Is.True);
        Assert.That(extent.Count, Is.EqualTo(1));
        Assert.That(extent[0].Tables, Is.EqualTo(4));
    }
}