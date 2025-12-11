using Main.Classes.Orders;
using Main.Classes.Restaurant;
using Menu;

namespace RestaurantTests;
using NUnit.Framework;
using Main.Classes.Employees;
using System;
using System.Linq;

public class AssociationTests
{
    [TestFixture]
    public class ManagerStaffAssociationTests
{
    [SetUp]
    public void Setup()
    {
        Manager.ClearExtentForTests();
    }

    [Test]
    public void AddManagedStaff_CreatesReverseConnection()
    {
        var senior = new Manager(1, "Derya", "Ogus", 80000, "IT", ManagerLevels.Senior);
        var junior = new Manager(2, "İbrahim", "Yeşil", 50000, "IT", ManagerLevels.Junior);
        
        senior.AddManagedStaff(junior);
        
        Assert.That(junior.Manager, Is.EqualTo(senior));
        Assert.That(senior.ManagedStaff.Contains(junior), Is.True);
    }

    [Test]
    public void RemoveManagedStaff_BreaksReverseConnection()
    {
        var manager = new Manager(1, "Derya", "Ogus", 80000, "IT", ManagerLevels.Senior);
        var staff = new Manager(2, "İbrahim", "Yeşil", 50000, "IT", ManagerLevels.Junior);
        
        manager.AddManagedStaff(staff);
        var result = manager.RemoveManagedStaff(staff);
        
        Assert.That(result, Is.True);
        Assert.That(staff.Manager, Is.Null);
        Assert.That(manager.ManagedStaff.Contains(staff), Is.False);
    }

    [Test]
    public void CannotManageHigherLevelManager()
    {
        var junior = new Manager(1, "İbrahim", "Yeşil", 50000, "IT", ManagerLevels.Junior);
        var senior = new Manager(2, "Derya", "Ogus", 80000, "IT", ManagerLevels.Senior);
        
        var ex = Assert.Throws<InvalidOperationException>(() => junior.AddManagedStaff(senior));
        Assert.That(ex.Message, Does.Contain("cannot manage"));
    }

    [Test]
    public void ReassigningStaff_RemovesFromOldManager()
    {
        var manager1 = new Manager(1, "Derya", "Ogus", 80000, "IT", ManagerLevels.Senior);
        var manager2 = new Manager(2, "Arda", "Seydol", 85000, "IT", ManagerLevels.Senior);
        var staff = new Manager(3, "İbrahim", "Yeşil", 50000, "IT", ManagerLevels.Junior);
        
        manager1.AddManagedStaff(staff);
        manager2.AddManagedStaff(staff);
        
        Assert.That(manager1.ManagedStaff.Contains(staff), Is.False);
        Assert.That(manager2.ManagedStaff.Contains(staff), Is.True);
        Assert.That(staff.Manager, Is.EqualTo(manager2));
    }

    [Test]
    public void CannotManageSelf()
    {
        var manager = new Manager(1, "Derya", "Ogus", 80000, "IT", ManagerLevels.Senior);
        
        var ex = Assert.Throws<InvalidOperationException>(() => manager.AddManagedStaff(manager));
        Assert.That(ex.Message, Is.EqualTo("Manager cannot manage themselves"));
    }
    
    [Test]
    public void DuplicateManagedStaff_ThrowsException()
    {
        var manager = new Manager(1, "Derya", "Ogus", 80000, "IT", ManagerLevels.Senior);
        var staff = new Manager(2, "İbrahim", "Yeşil", 50000, "IT", ManagerLevels.Junior);
        
        manager.AddManagedStaff(staff);
        
        var ex = Assert.Throws<ArgumentException>(() => manager.AddManagedStaff(staff));
        Assert.That(ex.Message, Does.Contain("already managed"));
    }
}
    [TestFixture]
    public class ShiftAssociationBagTests
    {
        [SetUp]
        public void Setup()
        {
            Restaurant.ClearExtentForTest();
            Manager.ClearExtentForTests();
        }

        [Test]
        public void Bag_AllowsMultipleShiftAssociations_SameRestaurantAndStaff()
        {
            var restaurant = new Restaurant(1, "Bella Vista", "11:00-22:00");
            var staff = new Manager(1, "Alice", "Smith", 70000, "Kitchen", ManagerLevels.Senior);
            
            var shift1 = ShiftAssociation.Create(staff, restaurant, ShiftType.Morning);
            var shift2 = ShiftAssociation.Create(staff, restaurant, ShiftType.Evening);
            var shift3 = ShiftAssociation.Create(staff, restaurant, ShiftType.Night);
            
            Assert.That(staff.ShiftAssociations.Count, Is.EqualTo(3));
            Assert.That(restaurant.ShiftAssociations.Count, Is.EqualTo(3));
        }

        [Test]
        public void ChangeShift_UpdatesCorrectly()
        {
            var restaurant = new Restaurant(1, "Bella Vista", "11:00-22:00");
            var staff = new Manager(1, "Derya", "Ogus", 60000, "Service", ManagerLevels.Junior);
            
            var shift = ShiftAssociation.Create(staff, restaurant, ShiftType.Morning);
            shift.ChangeShift(ShiftType.Afternoon);
            
            Assert.That(shift.ChangedShift, Is.EqualTo(ShiftType.Afternoon));
            Assert.That(shift.CurrentShift, Is.EqualTo(ShiftType.Morning)); 
        }

        [Test]
        public void RemoveShiftAssociation_BreaksConnections()
        {
            var restaurant = new Restaurant(1, "Bella Vista", "11:00-22:00");
            var staff = new Manager(1, "Arda", "Seydol", 55000, "HR", ManagerLevels.Junior);
            var shift = ShiftAssociation.Create(staff, restaurant, ShiftType.Morning);
            
            shift.Remove();
            
            Assert.That(staff.ShiftAssociations.Contains(shift), Is.False);
            Assert.That(restaurant.ShiftAssociations.Contains(shift), Is.False);
        }

        [Test]
        public void DeleteRestaurant_CascadesShiftAssociations()
        {
            var restaurant = new Restaurant(1, "Bella Vista", "11:00-22:00");
            var staff1 = new Manager(1, "Charlie", "Kirk", 70000, "Kitchen", ManagerLevels.Mid);
            var staff2 = new Manager(2, "Elon", "Musk", 65000, "Kitchen", ManagerLevels.Junior);
            
            ShiftAssociation.Create(staff1, restaurant, ShiftType.Morning);
            ShiftAssociation.Create(staff2, restaurant, ShiftType.Evening);
            
            restaurant.DeleteRestaurant();
            
            Assert.That(restaurant.ShiftAssociations.Count, Is.EqualTo(2));
            Assert.That(staff1.ShiftAssociations.Count, Is.EqualTo(1));
            Assert.That(staff2.ShiftAssociations.Count, Is.EqualTo(1));
        }
    }
    [TestFixture]
    public class MultipleAssociationsIntegrationTests
    {
        [SetUp]
        public void Setup()
        {
            Restaurant.ClearExtentForTest();
            Manager.ClearExtentForTests();
            Table.ClearExtentForTests();
        }

        [Test]
        public void ComplexScenario_AllAssociationsWorkTogether()
        {
            var restaurant = new Restaurant(1, "Bella Vista", "11:00-22:00");
            var manager = new Manager(1, "Alice", "Smith", 80000, "Management", ManagerLevels.Senior);
            var staff1 = new Manager(2, "Bob", "Jones", 50000, "Kitchen", ManagerLevels.Junior);
            var staff2 = new Manager(3, "Charlie", "Brown", 55000, "Service", ManagerLevels.Junior);
            
            manager.AddManagedStaff(staff1);
            manager.AddManagedStaff(staff2);
            
            var table1 = restaurant.AddTable(101, 1);
            var table2 = restaurant.AddTable(102, 2);
            
            var menu = restaurant.AddMenu(1, "Lunch Menu", "v1.0", true);
            
            
            var shift1 = ShiftAssociation.Create(staff1, restaurant, ShiftType.Morning);
            var shift2 = ShiftAssociation.Create(staff1, restaurant, ShiftType.Evening); 
            
            Assert.That(manager.ManagedStaff.Count, Is.EqualTo(2));
            Assert.That(restaurant.Tables.Count, Is.EqualTo(2));
            Assert.That(restaurant.Menus.Count, Is.EqualTo(1));
            Assert.That(restaurant.HireDates.Count, Is.EqualTo(0));
            Assert.That(restaurant.ShiftAssociations.Count, Is.EqualTo(2));
            
            Assert.That(staff1.Manager, Is.EqualTo(manager));
            Assert.That(table1.Restaurant, Is.EqualTo(restaurant));
            Assert.That(shift1.Staff, Is.EqualTo(staff1));
            
            restaurant.DeleteRestaurant();
            
            Assert.That(staff1.HireDates.Count, Is.EqualTo(0));
            Assert.That(staff1.ShiftAssociations.Count, Is.EqualTo(2));
        }
    }
    [TestFixture]
    public class RestaurantTableCompositionTests
    {
        [SetUp]
        public void Setup()
        {
            Restaurant.ClearExtentForTest();
            Table.ClearExtentForTests();
        }

        [Test]
        public void AddTable_CreatesReverseConnection()
        {
            var restaurant = new Restaurant(1, "Bella Vista", "11:00-22:00");
            var table = restaurant.AddTable(101, 1);
        
            Assert.That(restaurant.Tables.Contains(table), Is.True);
            Assert.That(table.Restaurant, Is.EqualTo(restaurant));
        }

        [Test]
        public void RemoveTable_BreaksConnection()
        {
            var restaurant = new Restaurant(1, "Bella Vista", "11:00-22:00");
            var table = restaurant.AddTable(101, 1);
        
            var result = restaurant.RemoveTable(table);
        
            Assert.That(result, Is.True);
            Assert.That(restaurant.Tables.Contains(table), Is.False);
            Assert.That(Table.GetExtent().Contains(table), Is.False);
        }

        [Test]
        public void DeleteRestaurant_CascadesDeleteTables()
        {
            var restaurant = new Restaurant(1, "Bella Vista", "11:00-22:00");
            var table1 = restaurant.AddTable(101, 1);
            var table2 = restaurant.AddTable(102, 2);
        
            restaurant.DeleteRestaurant();
        
            Assert.That(Restaurant.GetExtent().Contains(restaurant), Is.False);
            Assert.That(Table.GetExtent().Contains(table1), Is.True);
            Assert.That(Table.GetExtent().Contains(table2), Is.True);
        }

        [Test]
        public void DuplicateTable_ThrowsException()
        {
            var restaurant = new Restaurant(1, "Bella Vista", "11:00-22:00");
            restaurant.AddTable(101, 1);
        
            var ex = Assert.Throws<ArgumentException>(() => restaurant.AddTable(101, 2));
            Assert.That(ex.Message, Does.Contain("already exists"));
        }
    }
    
public class TestMenuItem : MenuItems
{
    public TestMenuItem(string name, decimal price, bool isAvailable, string? desc = null)
        : base(name, price, isAvailable, desc) { }
}

// ============================================
// CUSTOMER TESTS
// ============================================
[TestFixture]
public class CustomerTests
{
    [SetUp]
    public void Setup()
    {
        Customer.ClearExtentForTests();
    }

    [Test]
    public void AddOrder_CreatesReverseConnection()
    {
        var c = new Customer(1, "Ibrahim", "Yesil", "123456", "x@mail.com");
        var o = new Order(DateTime.Now, false, OrderStatus.Preparing, c);

        Assert.That(c.Orders.Contains(o), Is.True);
        Assert.That(o.Customer, Is.EqualTo(c));
    }

    [Test]
    public void Customer_CannotAddDuplicateOrder()
    {
        var c = new Customer(1, "Ibrahim", "Yesil", "123456", "x@mail.com");
        var o = new Order(DateTime.Now, false, OrderStatus.Preparing, c);

        c.AddOrder(o);

        Assert.That(c.Orders.Count, Is.EqualTo(1));
    }

    [Test]
    public void InvalidName_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new Customer(1, "", "Yesil", "123456", "a@a.com"));
    }

    [Test]
    public void InvalidEmail_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new Customer(1, "Ibrahim", "Yesil", "111111", "invalid"));
    }
}

// ============================================
// ORDER TESTS
// ============================================
[TestFixture]
public class OrderTests
{
    [SetUp]
    public void Setup()
    {
        Order.ClearExtentForTests();
    }

    [Test]
    public void Order_AssociationCreatesReverse()
    {
        var c = new Customer(1, "Ibrahim", "Yesil", "123", "x@x.com");
        var o = new Order(DateTime.Now, false, OrderStatus.Preparing, c);

        Assert.That(c.Orders.Contains(o), Is.True);
    }

    [Test]
    public void AddPayment_CreatesBidirectional()
    {
        var c = new Customer(1, "Test", "User", "123", "mail@mail.com");
        var o = new Order(DateTime.Now, false, OrderStatus.Preparing, c);

        var p = new Payment(20, DateTime.Now, 1, PaymentMethod.Cash, PaymentStatus.Pending, null, o);

        Assert.That(o.Payments.Contains(p), Is.True);
        Assert.That(p.Order, Is.EqualTo(o));
    }

    [Test]
    public void RemoveLastQuantity_Throws()
    {
        var c = new Customer(1, "A", "B", "111111", "a@a.com");
        var o = new Order(DateTime.Now, false, OrderStatus.Preparing, c);

        var item = new TestMenuItem("Pizza", 20, true);
        var q = new Quantity(o, item, 1);

        Assert.Throws<InvalidOperationException>(() => o.RemoveQuantity(q));
    }

    [Test]
    public void AddQuantity_IfQuantityBelongsToDifferentOrder_Throws()
    {
        var c = new Customer(1, "Test", "User", "111111", "x@mail.com");
        var o1 = new Order(DateTime.Now, false, OrderStatus.Preparing, c);
        var o2 = new Order(DateTime.Now, false, OrderStatus.Preparing, c);

        var item = new TestMenuItem("Soup", 10, true);

        Assert.Throws<InvalidOperationException>(() => new Quantity(o2, item, 1));
    }
}

// ============================================
// PAYMENT TESTS
// ============================================
[TestFixture]
public class PaymentTests
{
    [Test]
    public void Pay_SetsStatusAndPaidAt()
    {
        var c = new Customer(1, "Ibrahim", "Yesil", "123", "a@a.com");
        var o = new Order(DateTime.Now, false, OrderStatus.Preparing, c);

        var p = new Payment(30, DateTime.Now, 1, PaymentMethod.Card, PaymentStatus.Pending, null, o);

        p.Pay();

        Assert.That(p.Status, Is.EqualTo(PaymentStatus.Completed));
        Assert.That(p.PaidAt.HasValue, Is.True);
    }
}

// ============================================
// DELIVERY TESTS
// ============================================
[TestFixture]
public class DeliveryTests
{
    [SetUp]
    public void Setup()
    {
        Delivery.ClearExtentForTests();
    }

    [Test]
    public void Delivery_AssociationWithOrder_SetsReverse()
    {
        var c = new Customer(1, "Ibrahim", "Yesil", "999999", "x@mail.com");
        var o = new Order(DateTime.Now, false, OrderStatus.Preparing, c);

        var addr = new Adress("Street", "City", "12345");
        var d = new Delivery(1, DeliveryMethod.Courier, addr, DateTime.Now, null, 5, DeliveryStatus.Scheduled, o);

        Assert.That(o.Delivery, Is.EqualTo(d));
    }

    [Test]
    public void InvalidAddress_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new Adress("", "City", "12345"));
    }
}

// ============================================
// MENUITEMS + MENU + INGREDIENT TESTS
// ============================================
[TestFixture]
public class MenuItemsTests
{
    [SetUp]
    public void Setup()
    {
        MenuItems.ClearExtentForTests();
        Ingredient.ClearExtentForTests();
        Menu.Menu.ClearExtentForTest();
        Restaurant.ClearExtentForTest();
        Order.ClearExtentForTests();
        Customer.ClearExtentForTests();
        Delivery.ClearExtentForTests();
        Payment.ClearExtentForTests();
    }

    // -------------------- QUANTITY TESTS --------------------

    [Test]
    public void Quantity_AddsBothSides()
    {
        var c = new Customer(1, "Test", "User", "123456", "xx@mail.com");
        var o = new Order(DateTime.Now, false, OrderStatus.Preparing, c);
        var item = new TestMenuItem("Burger", 25m, true);

        var q = new Quantity(o, item, 2);

        Assert.That(item.Quantities.Contains(q), Is.True);
        Assert.That(o.Quantities.Contains(q), Is.True);
    }

    [Test]
    public void Quantity_MustBePositive()
    {
        var c = new Customer(1, "A", "B", "123", "a@b.com");
        var o = new Order(DateTime.Now, false, OrderStatus.Preparing, c);
        var item = new TestMenuItem("Pizza", 30m, true);

        Assert.Throws<ArgumentException>(() => new Quantity(o, item, 0));
    }

    [Test]
    public void AddQuantity_DifferentMenuItem_Throws()
    {
        var c = new Customer(1, "X", "Y", "111", "a@mail.com");
        var o = new Order(DateTime.Now, false, OrderStatus.Preparing, c);

        var item1 = new TestMenuItem("Item1", 10m, true);
        var item2 = new TestMenuItem("Item2", 10m, true);

        var q = new Quantity(o, item1, 1);

        Assert.Throws<InvalidOperationException>(() => item2.AddQuantity(q));
    }

    [Test]
    public void AddQuantity_Duplicate_Throws()
    {
        var c = new Customer(1, "T", "U", "222", "t@mail.com");
        var o = new Order(DateTime.Now, false, OrderStatus.Preparing, c);

        var item = new TestMenuItem("Wrap", 15m, true);
        var q = new Quantity(o, item, 1);

        Assert.Throws<InvalidOperationException>(() => item.AddQuantity(q));
    }

    // -------------------- MENU RELATION TESTS --------------------

    [Test]
    public void AddMenu_CreatesReverseConnection()
    {
        var item = new TestMenuItem("Soup", 10m, true);
        var rest = new Main.Classes.Restaurant.Restaurant(1, "Bella", "10-22");
        var menu = new Menu.Menu(1, "Main Menu", "v1", true, rest);

        item.AddMenu(menu);

        Assert.That(item.Menus.Contains(menu), Is.True);
        Assert.That(menu.Items.Contains(item), Is.True);
    }

    [Test]
    public void AddMenu_Duplicate_Throws()
    {
        var item = new TestMenuItem("Soup", 10m, true);
        var rest = new Main.Classes.Restaurant.Restaurant(1, "Bella", "10-22");
        var menu = new Menu.Menu(1, "Main Menu", "v1", true, rest);

        item.AddMenu(menu);

        Assert.Throws<InvalidOperationException>(() => item.AddMenu(menu));
    }

    // -------------------- INGREDIENT RELATION TESTS --------------------

    [Test]
    public void AddIngredient_CreatesBidirectional()
    {
        var item = new TestMenuItem("Salad", 12m, true);
        var ing = new Ingredient(1, "Tomato", Unit.Gram);

        item.AddIngredient(ing);

        Assert.That(item.Ingredients.Contains(ing), Is.True);
        Assert.That(ing.MenuItems.Contains(item), Is.True);
    }

    [Test]
    public void AddIngredient_Duplicate_Throws()
    {
        var item = new TestMenuItem("Salad", 12m, true);
        var ing = new Ingredient(1, "Onion", Unit.Gram);

        item.AddIngredient(ing);

        Assert.Throws<InvalidOperationException>(() => item.AddIngredient(ing));
    }

    // -------------------- ATTRIBUTE VALIDATION --------------------

    [Test]
    public void InvalidName_Throws()
    {
        Assert.Throws<ArgumentException>(() => new TestMenuItem("", 10m, true));
        Assert.Throws<ArgumentException>(() => new TestMenuItem("A", 10m, true));
        Assert.Throws<ArgumentException>(() => new TestMenuItem(new string('x', 60), 10m, true));
    }

    [Test]
    public void InvalidPrice_Throws()
    {
        Assert.Throws<ArgumentException>(() => new TestMenuItem("Tea", -1m, true));
        Assert.Throws<ArgumentException>(() => new TestMenuItem("Tea", 1500m, true));
    }

    [Test]
    public void InvalidDescription_Throws()
    {
        Assert.Throws<ArgumentException>(() => new TestMenuItem("Tea", 10m, true, ""));
    }
}

    
}