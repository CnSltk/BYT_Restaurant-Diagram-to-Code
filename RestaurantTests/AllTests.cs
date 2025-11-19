using Menu;
using Main.Classes.Employees;
using Main.Classes.Orders;
using Main.Classes.Menu;



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
   public void Name_EmptyValue_ThrowsException()
   {
       var item = new TestableMenuItem("Pizza", 10, true);

       var ex = Assert.Throws<ArgumentException>(() =>
           item.Name = ""
       );

       Assert.That(ex.Message, Is.EqualTo("Name cannot be empty"));
   }


    [Test]
    public void UpdateMenuItem_UpdatesAllFields()
    {
        var item = new TestableMenuItem("Burger", 18, true);

        item.UpdateMenuItem(
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
    
    [Test]
    public void Name_TooLong_ThrowsException()
    {
        var item = new TestableMenuItem("Pizza", 20m, true);
        var longName = new string('A', 51);

        var ex = Assert.Throws<ArgumentException>(() => item.Name = longName);

        Assert.That(ex!.Message, Is.EqualTo("Name length must be between 2 and 50 characters"));
    }

    [Test]
    public void AddAllergen_DuplicateAllergen_ThrowsException()
    {
        var item = new TestableMenuItem("Pizza", 20m, true);
        item.AddAllergen("Gluten");

        var ex = Assert.Throws<ArgumentException>(() => item.AddAllergen("Gluten"));

        Assert.That(ex!.Message, Is.EqualTo("Allergen already exists"));
    }

    [Test]
    public void SaveAndLoadExtent_RestoresMenuItems()
    {
        MenuItems.ClearExtent();

        var i1 = new TestableMenuItem("Soup", 10m, true);
        var i2 = new TestableMenuItem("Salad", 15m, true);

        MenuItems.CreateMenuItem(i1);
        MenuItems.CreateMenuItem(i2);

        var path = "MenuItems_Test.xml";
        if (File.Exists(path))
            File.Delete(path);

        MenuItems.SaveExtent(path);
        MenuItems.ClearExtent();
        MenuItems.LoadExtent(path);

        Assert.That(MenuItems.Extent.Count, Is.EqualTo(2));
        Assert.That(MenuItems.Extent.Any(i => i.Name == "Soup"));
        Assert.That(MenuItems.Extent.Any(i => i.Name == "Salad"));
    }

}

/* -------------------------------------------------------------
   MENU TEST
--------------------------------------------------------------*/
[TestFixture]
public class MenuTests
{
    [Test]
    public void Menu_ValidValues_AssignedCorrectly()
    {
        var menu = new Menu.Menu("Lunch Menu", "v1.0", true);

        Assert.That(menu.Name, Is.EqualTo("Lunch Menu"));
        Assert.That(menu.Version, Is.EqualTo("v1.0"));
        Assert.That(menu.IsActive, Is.True);
    }

    [Test]
    public void Menu_NameTooLong_ThrowsException()
    {
        var menu = new Menu.Menu("Lunch Menu", "v1.0", true);
        var longName = new string('A', 51);

        var ex = Assert.Throws<ArgumentException>(() => menu.Name = longName);

        Assert.That(ex!.Message, Is.EqualTo("Menu name length must be between 2 and 50 characters"));
    }

    [Test]
    public void Menu_SaveAndLoadExtent_RestoresMenus()
    {
        Menu.Menu.ClearExtentForTest();

        var m1 = new Menu.Menu("Lunch Menu", "v1.0", true);
        var m2 = new Menu.Menu("Dinner Menu", "v2.0", false);

        var path = "Menu_Test.json";
        if (File.Exists(path))
            File.Delete(path);

        Menu.Menu.Save(path);
        Menu.Menu.ClearExtentForTest();
        Menu.Menu.Load(path);

        Assert.That(Menu.Menu.GetExtent().Count, Is.EqualTo(2));
        Assert.That(Menu.Menu.GetExtent().Any(m => m.Name == "Lunch Menu"));
        Assert.That(Menu.Menu.GetExtent().Any(m => m.Name == "Dinner Menu"));
    }
}
/* -------------------------------------------------------------
   FOOD TESTS
--------------------------------------------------------------*/
[TestFixture]
public class FoodTests
{
    [Test]
    public void Food_ValidValues_AssignedCorrectly()
    {
        var food = new Food(
            name: "Pasta",
            price: 30m,
            isAvailable: true,
            spiceLevel: true,
            isVegetarian: true,
            calories: 600,
            prepTimeMin: 20,
            category: FoodCategory.MainCourse,
            description: "Creamy pasta"
        );

        Assert.That(food.Name, Is.EqualTo("Pasta"));
        Assert.That(food.Price, Is.EqualTo(30m));
        Assert.That(food.Calories, Is.EqualTo(600));
        Assert.That(food.PrepTimeMin, Is.EqualTo(20));
        Assert.That(food.Category, Is.EqualTo(FoodCategory.MainCourse));
        Assert.That(food.IsVegetarian, Is.True);
        Assert.That(food.SpiceLevel, Is.True);
    }

    [Test]
    public void Food_CaloriesOutOfRange_ThrowsException()
    {
        var food = new Food(
            name: "Soup",
            price: 15m,
            isAvailable: true,
            spiceLevel: false,
            isVegetarian: true,
            calories: 200,
            prepTimeMin: 10,
            category: FoodCategory.Starter
        );

        Assert.Throws<ArgumentException>(() => food.Calories = 0);
        Assert.Throws<ArgumentException>(() => food.Calories = 6000);
    }

    [Test]
    public void Food_SaveAndLoadExtent_RestoresFoods()
    {
        Food.ClearExtentForTests();

        var f1 = new Food("Soup", 12m, true, false, true, 200, 10, FoodCategory.Starter);
        var f2 = new Food("Cake", 18m, true, false, false, 500, 30, FoodCategory.Dessert);

        var path = "Food_Test.json";
        if (File.Exists(path))
            File.Delete(path);

        Food.Save(path);
        Food.ClearExtentForTests();
        Assert.That(Food.GetExtent().Count, Is.EqualTo(0));

        Food.Load(path);

        Assert.That(Food.GetExtent().Count, Is.EqualTo(2));
        Assert.That(Food.GetExtent().Any(f => f.Name == "Soup"));
        Assert.That(Food.GetExtent().Any(f => f.Name == "Cake"));
    }
}
/* -------------------------------------------------------------
   BEVERAGE TESTS
--------------------------------------------------------------*/
[TestFixture]
public class BeverageTests
{
    [Test]
    public void Beverage_ValidValues_AssignedCorrectly()
    {
        var drink = new Beverage(
            name: "Cola",
            price: 8m,
            isAvailable: true,
            volumeMl: 500,
            category: BeverageCategory.SoftBeverage
        );

        Assert.That(drink.Name, Is.EqualTo("Cola"));
        Assert.That(drink.Price, Is.EqualTo(8m));
        Assert.That(drink.VolumeMl, Is.EqualTo(500));
        Assert.That(drink.Category, Is.EqualTo(BeverageCategory.SoftBeverage));
    }

    [Test]
    public void Beverage_InvalidVolume_ThrowsException()
    {
        var drink = new Beverage(
            name: "Water",
            price: 5m,
            isAvailable: true,
            volumeMl: 500,
            category: BeverageCategory.SoftBeverage
        );

        var ex = Assert.Throws<ArgumentException>(() => drink.VolumeMl = 750);

        Assert.That(ex!.Message, Is.EqualTo("Volume must be 500 ml or 1000 ml"));
    }

    [Test]
    public void Beverage_SaveAndLoadExtent_RestoresBeverages()
    {
        Beverage.ClearExtentForTests();

        var b1 = new Beverage("Tea", 6m, true, 500, BeverageCategory.HotBeverage);
        var b2 = new Beverage("Beer", 15m, true, 1000, BeverageCategory.Alcoholic);

        var path = "Beverage_Test.json";
        if (File.Exists(path))
            File.Delete(path);

        Beverage.Save(path);
        Beverage.ClearExtentForTests();
        Assert.That(Beverage.GetExtent().Count, Is.EqualTo(0));

        Beverage.Load(path);

        Assert.That(Beverage.GetExtent().Count, Is.EqualTo(2));
        Assert.That(Beverage.GetExtent().Any(b => b.Name == "Tea"));
        Assert.That(Beverage.GetExtent().Any(b => b.Name == "Beer"));
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
            name: "Tomato",
            unit: Unit.Gram,
            allergens: new[] { "None" }
        );

        Assert.That(ingredient.Name, Is.EqualTo("Tomato"));
        Assert.That(ingredient.Unit, Is.EqualTo(Unit.Gram));
        Assert.That(ingredient.Allergens.Count, Is.EqualTo(1));
        Assert.That(ingredient.Allergens.Contains("None"), Is.True);
    }

    [Test]
    public void Ingredient_AddAllergen_Empty_ThrowsException()
    {
        var ingredient = new Ingredient(
            name: "Cheese",
            unit: Unit.Gram,
            allergens: null
        );

        var ex = Assert.Throws<ArgumentException>(() => ingredient.AddAllergen(" "));

        Assert.That(ex!.Message, Is.EqualTo("Allergen cannot be empty"));
    }

    [Test]
    public void Ingredient_SaveAndLoadExtent_RestoresIngredients()
    {
        Ingredient.ClearExtentForTests();

        var i1 = new Ingredient("Onion", Unit.Gram, null);
        var i2 = new Ingredient("Milk", Unit.Ml, new[] { "Lactose" });

        var path = "Ingredient_Test.json";
        if (File.Exists(path))
            File.Delete(path);

        Ingredient.Save(path);

        Ingredient.ClearExtentForTests();
        Assert.That(Ingredient.GetExtent().Count, Is.EqualTo(0));

        Ingredient.Load(path);

        Assert.That(Ingredient.GetExtent().Count, Is.EqualTo(2));
        Assert.That(Ingredient.GetExtent().Any(i => i.Name == "Onion"));
        Assert.That(Ingredient.GetExtent().Any(i => i.Name == "Milk"));
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


