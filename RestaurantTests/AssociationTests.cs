using Main.Classes.Restaurant;

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
            
            // Create multiple shift associations (Bag allows this)
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
}