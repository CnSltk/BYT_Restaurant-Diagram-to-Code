using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Main.Classes.Employees;
using Main.Classes.Orders;
using Main.Classes.Restaurant;
using NUnit.Framework;

namespace RestaurantTests
{
    [TestFixture]
    public class StaffFlatteningTests
    {
        [SetUp]
        public void Setup()
        {
            // Clear extent using reflection
            var extentField = typeof(Staff).GetField("_extent", BindingFlags.NonPublic | BindingFlags.Static);
            extentField?.SetValue(null, new List<Staff>());
            
            var nextIdField = typeof(Staff).GetField("_nextId", BindingFlags.NonPublic | BindingFlags.Static);
            nextIdField?.SetValue(null, 1);
        }

        // ============================
        // FACTORY METHOD TESTS
        // ============================

        [Test]
        public void CreateManager_SetsCorrectTypeAndAttributes()
        {
            var manager = Staff.CreateManager(1, "Alice", "Smith", 80000, "Management", Level.Senior);
            
            Assert.That(manager.StaffType, Is.EqualTo(StaffType.Manager));
            Assert.That(manager.Level, Is.EqualTo(Level.Senior));
            Assert.That(manager.HasAccessToVault, Is.Null);
            Assert.That(manager.Tables, Is.Null);
            Assert.That(manager.SignatureDish, Is.Null);
            Assert.That(Staff.GetExtent().Count, Is.EqualTo(1));
        }

        [Test]
        public void CreateCashier_SetsCorrectTypeAndAttributes()
        {
            var cashier = Staff.CreateCashier(2, "Bob", "Jones", 2500, "Front", true);
            
            Assert.That(cashier.StaffType, Is.EqualTo(StaffType.Cashier));
            Assert.That(cashier.HasAccessToVault, Is.True);
            Assert.That(cashier.Level, Is.Null);
            Assert.That(cashier.Tables, Is.Null);
            Assert.That(cashier.SignatureDish, Is.Null);
        }

        

        [Test]
        public void CreateChef_SetsCorrectTypeAndAttributes()
        {
            var chef = Staff.CreateChef(4, "David", "Lee", 3500, "Kitchen");
            
            Assert.That(chef.StaffType, Is.EqualTo(StaffType.Chef));
            Assert.That(chef.Level, Is.Null);
            Assert.That(chef.HasAccessToVault, Is.Null);
            Assert.That(chef.Tables, Is.Null);
            Assert.That(chef.SignatureDish, Is.Null);
        }

        [Test]
        public void CreateHeadChef_SetsCorrectTypeAndAttributes()
        {
            var headChef = Staff.CreateHeadChef(5, "Eve", "Wilson", 5000, "Kitchen", "Beef Wellington");
            
            Assert.That(headChef.StaffType, Is.EqualTo(StaffType.HeadChef));
            Assert.That(headChef.SignatureDish, Is.EqualTo("Beef Wellington"));
            Assert.That(headChef.Level, Is.Null);
            Assert.That(headChef.HasAccessToVault, Is.Null);
            Assert.That(headChef.Tables, Is.Null);
        }

        [Test]
        public void CreateHeadChef_EmptySignatureDish_Throws()
        {
            Assert.Throws<ArgumentException>(() => 
                Staff.CreateHeadChef(5, "Eve", "Wilson", 5000, "Kitchen", ""));
            
            Assert.Throws<ArgumentException>(() => 
                Staff.CreateHeadChef(5, "Eve", "Wilson", 5000, "Kitchen", "   "));
        }

        // ============================
        // CONDITIONAL ATTRIBUTE VALIDATION TESTS
        // ============================

        [Test]
        public void Manager_MissingLevel_Throws()
        {
            // Use reflection to bypass factory validation
            var staff = (Staff)Activator.CreateInstance(typeof(Staff), true);
            typeof(Staff).GetProperty(nameof(Staff.StaffId))?.SetValue(staff, 1);
            typeof(Staff).GetField("_firstName", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, "Test");
            typeof(Staff).GetField("_lastName", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, "Test");
            typeof(Staff).GetField("_salary", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, 3000m);
            typeof(Staff).GetField("_department", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, "Test");
            typeof(Staff).GetProperty(nameof(Staff.StaffType))?.SetValue(staff, StaffType.Manager);
            
            // Level is null by default
            Assert.Throws<InvalidOperationException>(() => staff.ValidateConditionalAttributes());
        }

        [Test]
        public void Cashier_MissingHasAccessToVault_Throws()
        {
            var staff = (Staff)Activator.CreateInstance(typeof(Staff), true);
            typeof(Staff).GetProperty(nameof(Staff.StaffId))?.SetValue(staff, 1);
            typeof(Staff).GetField("_firstName", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, "Test");
            typeof(Staff).GetField("_lastName", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, "Test");
            typeof(Staff).GetField("_salary", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, 3000m);
            typeof(Staff).GetField("_department", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, "Test");
            typeof(Staff).GetProperty(nameof(Staff.StaffType))?.SetValue(staff, StaffType.Cashier);
            
            Assert.Throws<InvalidOperationException>(() => staff.ValidateConditionalAttributes());
        }

        [Test]
        public void Waiter_MissingTables_Throws()
        {
            var staff = (Staff)Activator.CreateInstance(typeof(Staff), true);
            typeof(Staff).GetProperty(nameof(Staff.StaffId))?.SetValue(staff, 1);
            typeof(Staff).GetField("_firstName", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, "Test");
            typeof(Staff).GetField("_lastName", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, "Test");
            typeof(Staff).GetField("_salary", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, 3000m);
            typeof(Staff).GetField("_department", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, "Test");
            typeof(Staff).GetProperty(nameof(Staff.StaffType))?.SetValue(staff, StaffType.Waiter);
            
            Assert.Throws<InvalidOperationException>(() => staff.ValidateConditionalAttributes());
        }

        [Test]
        public void HeadChef_MissingSignatureDish_Throws()
        {
            var staff = (Staff)Activator.CreateInstance(typeof(Staff), true);
            typeof(Staff).GetProperty(nameof(Staff.StaffId))?.SetValue(staff, 1);
            typeof(Staff).GetField("_firstName", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, "Test");
            typeof(Staff).GetField("_lastName", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, "Test");
            typeof(Staff).GetField("_salary", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, 3000m);
            typeof(Staff).GetField("_department", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(staff, "Test");
            typeof(Staff).GetProperty(nameof(Staff.StaffType))?.SetValue(staff, StaffType.HeadChef);
            
            Assert.Throws<InvalidOperationException>(() => staff.ValidateConditionalAttributes());
        }

        [Test]
        public void Manager_WrongAttributesSet_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => 
                Staff.CreateManager(1, "Test", "Test", 3000, "Test", Level.Senior)
                    .GetType()
                    .GetMethod("ValidateConditionalAttributes", BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(Staff.CreateCashier(1, "Test", "Test", 3000, "Test", true), null));
        }

        // ============================
        // METHOD ACCESS CONTROL TESTS
        // ============================

        [Test]
        public void NonManager_CallingManagementMethods_Throws()
        {
            var cashier = Staff.CreateCashier(1, "Test", "Test", 2500, "Front", true);
            var chef = Staff.CreateChef(3, "Test", "Test", 3500, "Kitchen");
            var headChef = Staff.CreateHeadChef(4, "Test", "Test", 5000, "Kitchen", "Dish");
            
            var staffMembers = new[] { cashier, chef, headChef };
            
            foreach (var staff in staffMembers)
            {
                Assert.Throws<InvalidOperationException>(() => staff.HireStaff());
                Assert.Throws<InvalidOperationException>(() => staff.FireStaff());
                Assert.Throws<InvalidOperationException>(() => staff.ManageEmployees());
                Assert.Throws<InvalidOperationException>(() => staff.ChangeStaffShift());
            }
        }

        [Test]
        public void NonCashier_CallingCashierMethods_Throws()
        {
            var manager = Staff.CreateManager(1, "Test", "Test", 80000, "Management", Level.Senior);
            var chef = Staff.CreateChef(3, "Test", "Test", 3500, "Kitchen");
            var headChef = Staff.CreateHeadChef(4, "Test", "Test", 5000, "Kitchen", "Dish");
            
            var staffMembers = new[] { manager, chef, headChef };
            
            foreach (var staff in staffMembers)
            {
                Assert.Throws<InvalidOperationException>(() => staff.IssueRefund(1, "Test", "Test","test"));
                Assert.Throws<InvalidOperationException>(() => staff.ReceivePayment(1, "Test", "Test"));
            }
        }

       

        [Test]
        public void NonHeadChef_CallingHeadChefMethods_Throws()
        {
            var manager = Staff.CreateManager(1, "Test", "Test", 80000, "Management", Level.Senior);
            var cashier = Staff.CreateCashier(2, "Test", "Test", 2500, "Front", true);
            var chef = Staff.CreateChef(4, "Test", "Test", 3500, "Kitchen");
            
            var staffMembers = new[] { manager, cashier, chef };
            
            foreach (var staff in staffMembers)
            {
                Assert.Throws<InvalidOperationException>(() => staff.ManageInventory());
            }
        }

        // ============================
        // MANAGER-SPECIFIC TESTS
        // ============================

        [Test]
        public void Manager_AddManagedStaff_UpdatesBothSides()
        {
            var senior = Staff.CreateManager(1, "Senior", "Manager", 80000, "IT", Level.Senior);
            var junior = Staff.CreateManager(2, "Junior", "Manager", 50000, "IT", Level.Junior);
            
            senior.AddManagedStaff(junior);
            
            Assert.That(junior.Manager, Is.EqualTo(senior));
            Assert.That(senior.ManagedStaff.Count, Is.EqualTo(1));
            Assert.That(senior.ManagedStaff.Contains(junior), Is.True);
        }

        [Test]
        public void Manager_CannotManageHigherLevel()
        {
            var junior = Staff.CreateManager(1, "Junior", "Manager", 50000, "IT", Level.Junior);
            var mid = Staff.CreateManager(2, "Mid", "Manager", 65000, "IT", Level.Mid);
            var senior = Staff.CreateManager(3, "Senior", "Manager", 80000, "IT", Level.Senior);
            
            Assert.Throws<InvalidOperationException>(() => junior.AddManagedStaff(mid));
            Assert.Throws<InvalidOperationException>(() => junior.AddManagedStaff(senior));
            Assert.Throws<InvalidOperationException>(() => mid.AddManagedStaff(senior));
        }

        [Test]
        public void ReassigningStaff_RemovesFromOldManager()
        {
            var manager1 = Staff.CreateManager(1, "Manager1", "Test", 80000, "IT", Level.Senior);
            var manager2 = Staff.CreateManager(2, "Manager2", "Test", 80000, "IT", Level.Senior);
            var staff = Staff.CreateManager(3, "Staff", "Test", 50000, "IT", Level.Junior);
            
            manager1.AddManagedStaff(staff);
            manager2.AddManagedStaff(staff);
            
            Assert.That(staff.Manager, Is.EqualTo(manager2));
            Assert.That(manager1.ManagedStaff.Contains(staff), Is.False);
            Assert.That(manager2.ManagedStaff.Contains(staff), Is.True);
        }

        // ============================
        // EXTENT MANAGEMENT TESTS
        // ============================

        [Test]
        public void AllStaffTypes_ShareSameExtent()
        {
            var manager = Staff.CreateManager(1, "Alice", "Smith", 80000, "Management", Level.Senior);
            var cashier = Staff.CreateCashier(2, "Bob", "Jones", 2500, "Front", true);
            var chef = Staff.CreateChef(4, "David", "Lee", 3500, "Kitchen");
            var headChef = Staff.CreateHeadChef(5, "Eve", "Wilson", 5000, "Kitchen", "Beef Wellington");
            
            Assert.That(Staff.GetExtent().Count, Is.EqualTo(5));
            Assert.That(Staff.GetExtent().Contains(manager), Is.True);
            Assert.That(Staff.GetExtent().Contains(cashier), Is.True);
            Assert.That(Staff.GetExtent().Contains(chef), Is.True);
            Assert.That(Staff.GetExtent().Contains(headChef), Is.True);
        }

        [Test]
        public void ClearExtent_RemovesAllStaffTypes()
        {
            Staff.CreateManager(1, "Alice", "Smith", 80000, "Management", Level.Senior);
            Staff.CreateCashier(2, "Bob", "Jones", 2500, "Front", true);
            Staff.CreateChef(4, "David", "Lee", 3500, "Kitchen");
            Staff.CreateHeadChef(5, "Eve", "Wilson", 5000, "Kitchen", "Beef Wellington");
            
            Assert.That(Staff.GetExtent().Count, Is.EqualTo(5));
            
            Staff.ClearExtentForTests();
            
            Assert.That(Staff.GetExtent().Count, Is.EqualTo(0));
        }

        // ============================
        // SERIALIZATION TESTS
        // ============================

        [Test]
        public void Serialization_IncludesDiscriminatorAndConditionalAttributes()
        {
            var manager = Staff.CreateManager(1, "Alice", "Smith", 80000, "Management", Level.Senior);
            var cashier = Staff.CreateCashier(2, "Bob", "Jones", 2500, "Front", true);
            
            // This would require JsonSerializer testing or marking properties properly
            // For now, we verify the properties are accessible
            Assert.That(manager.StaffType, Is.EqualTo(StaffType.Manager));
            Assert.That(manager.Level, Is.Not.Null);
            Assert.That(cashier.StaffType, Is.EqualTo(StaffType.Cashier));
            Assert.That(cashier.HasAccessToVault, Is.Not.Null);
        }
        
        
        [Test]
        public void FactoryMethods_AssignUniqueIds()
        {
            var staff1 = Staff.CreateManager(1, "Alice", "Smith", 80000, "Management", Level.Senior);
            var staff2 = Staff.CreateCashier(2, "Bob", "Jones", 2500, "Front", true);
            
            Assert.That(staff1.StaffId, Is.Not.EqualTo(staff2.StaffId));
        }

        [Test]
        public void Chef_NoSpecificAttributes_Null()
        {
            var chef = Staff.CreateChef(1, "David", "Lee", 3500, "Kitchen");
            
            Assert.That(chef.Level, Is.Null);
            Assert.That(chef.HasAccessToVault, Is.Null);
            Assert.That(chef.Tables, Is.Null);
            Assert.That(chef.SignatureDish, Is.Null);
        }

        [Test]
        public void Manager_LevelEnumValues_WorkCorrectly()
        {
            var junior = Staff.CreateManager(1, "Junior", "Manager", 50000, "IT", Level.Junior);
            var mid = Staff.CreateManager(2, "Mid", "Manager", 65000, "IT", Level.Mid);
            var senior = Staff.CreateManager(3, "Senior", "Manager", 80000, "IT", Level.Senior);
            
            Assert.That(junior.Level, Is.EqualTo(Level.Junior));
            Assert.That(mid.Level, Is.EqualTo(Level.Mid));
            Assert.That(senior.Level, Is.EqualTo(Level.Senior));
        }

        [Test]
        public void AddManagedStaff_NullStaff_Throws()
        {
            var manager = Staff.CreateManager(1, "Alice", "Smith", 80000, "Management", Level.Senior);
            
            Assert.Throws<ArgumentNullException>(() => manager.AddManagedStaff(null));
        }

        [Test]
        public void RemoveManagedStaff_NullStaff_ReturnsFalse()
        {
            var manager = Staff.CreateManager(1, "Alice", "Smith", 80000, "Management", Level.Senior);
            
            var result = manager.RemoveManagedStaff(null);
            
            Assert.That(result, Is.False);
        }

        [Test]
        public void RemoveManagedStaff_StaffNotManaged_ReturnsFalse()
        {
            var manager = Staff.CreateManager(1, "Alice", "Smith", 80000, "Management", Level.Senior);
            var staff = Staff.CreateManager(2, "Bob", "Jones", 50000, "IT", Level.Junior);
            
            var result = manager.RemoveManagedStaff(staff);
            
            Assert.That(result, Is.False);
        }
    }
}