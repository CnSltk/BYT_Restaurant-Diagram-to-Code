using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Main.Classes.Employees;
using NUnit.Framework;

namespace RestaurantTests
{
    [TestFixture]
    public class CashierTests
    {
        private string _testFilePath;
        
        [SetUp]
        public void Setup()
        {
            // Clear Staff extent using reflection
            var extentField = typeof(Staff).GetField("_extent", BindingFlags.NonPublic | BindingFlags.Static);
            extentField?.SetValue(null, new List<Staff>());
            
            // Reset next ID
            var nextIdField = typeof(Staff).GetField("_nextId", BindingFlags.NonPublic | BindingFlags.Static);
            nextIdField?.SetValue(null, 1);
            
            _testFilePath = Path.Combine(Path.GetTempPath(), $"staff_test_{Guid.NewGuid()}.json");
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
            Assert.Throws<ArgumentException>(() => 
                Staff.CreateCashier(1, null, "Ogus", 3000m, "Front", true));
        }

        [Test]
        public void Constructor_EmptyLastName()
        {
            Assert.Throws<ArgumentException>(() => 
                Staff.CreateCashier(2, "Derya", "", 3000m, "Front", true));
        }

        [Test]
        public void Constructor_NegativeSalary()
        {
            Assert.Throws<ArgumentException>(() => 
                Staff.CreateCashier(3, "Derya", "Ogus", -100m, "Front", true));
        }

        [Test]
        public void ReceivePayment_InvalidAmount()
        {
            var cashier = Staff.CreateCashier(4, "Derya", "Ogus", 3000m, "Front", true);
            
            Assert.Throws<ArgumentException>(() => cashier.ReceivePayment(0m, "Cash", "ORDER-123"));
            Assert.Throws<ArgumentException>(() => cashier.ReceivePayment(-10m, "Cash", "ORDER-123"));
        }

        [Test]
        public void ReceivePayment_NullPaymentMethod()
        {
            var cashier = Staff.CreateCashier(5, "Derya", "Ogus", 3000m, "Front", true);
            
            Assert.Throws<ArgumentException>(() => cashier.ReceivePayment(50m, null, "ORDER-123"));
            Assert.Throws<ArgumentException>(() => cashier.ReceivePayment(50m, "", "ORDER-123"));
            Assert.Throws<ArgumentException>(() => cashier.ReceivePayment(50m, "   ", "ORDER-123"));
        }

        [Test]
        public void IssueRefund_InvalidAmount()
        {
            var cashier = Staff.CreateCashier(6, "Derya", "Ogus", 3000m, "Front", true);
            
            Assert.Throws<ArgumentException>(() => cashier.IssueRefund(0m, "ORDER-123", "Reason", "AUTH-456"));
            Assert.Throws<ArgumentException>(() => cashier.IssueRefund(-10m, "ORDER-123", "Reason", "AUTH-456"));
        }

        [Test]
        public void Save_WithValidData()
        {
            Staff.CreateCashier(7, "Derya", "Ogus", 3000m, "Front", true);
            Staff.CreateCashier(8, "Can", "Saltik", 3200m, "Front", false);
            
            Staff.Save(_testFilePath);
            
            Assert.That(File.Exists(_testFilePath), Is.True);
            var json = File.ReadAllText(_testFilePath);
            Assert.That(json, Does.Contain("Derya"));
            Assert.That(json, Does.Contain("Ogus"));
            Assert.That(json, Does.Contain("3000"));
            Assert.That(json, Does.Contain("Can"));
            Assert.That(json, Does.Contain("Saltik"));
            Assert.That(json, Does.Contain("Cashier"));
        }

        [Test]
        public void Load_WithValidFile()
        {
            var extentField = typeof(Staff).GetField("_extent", BindingFlags.NonPublic | BindingFlags.Static);
            extentField?.SetValue(null, new List<Staff>());
            
            var testData = @"[
  {
    ""StaffId"": 1,
    ""FirstName"": ""Derya"",
    ""LastName"": ""Ogus"",
    ""Salary"": 3100,
    ""Department"": ""Front"",
    ""StaffType"": 1,
    ""HasAccessToVault"": true
  }
]";
            File.WriteAllText(_testFilePath, testData);
            
            bool result = Staff.Load(_testFilePath);
            var extent = Staff.GetExtent();
            
            Assert.That(result, Is.True);
            Assert.That(extent.Count, Is.EqualTo(1));
        }

        [Test]
        public void Load_WithNonExistentFile()
        {
            Staff.CreateCashier(9, "Derya", "Ogus", 3000m, "Front", true);
            
            bool result = Staff.Load("nonexistent.json");
            
            Assert.That(result, Is.False);
            Assert.That(Staff.GetExtent().Count, Is.EqualTo(0));
        }

        [Test]
        public void Load_WithInvalidJson()
        {
            File.WriteAllText(_testFilePath, "invalid json content");
            
            bool result = Staff.Load(_testFilePath);
            
            Assert.That(result, Is.False);
            Assert.That(Staff.GetExtent().Count, Is.EqualTo(0));
        }
    }
}