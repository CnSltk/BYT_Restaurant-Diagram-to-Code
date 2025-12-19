using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Main.Classes.Restaurant;
using Menu;

namespace Main.Classes.Employees;

public enum StaffType
{
    Manager,
    Cashier,
    Waiter,
    Chef,
    HeadChef
}

public enum Level
{
    Junior,
    Mid,
    Senior
}


[Serializable]
public class Staff
{
    
    private readonly List<HireDate> _hireDates = new();
    public IReadOnlyCollection<HireDate> HireDates => _hireDates.AsReadOnly();
    
    private readonly List<ShiftAssociation> _shiftAssociations = new();
    public IReadOnlyCollection<ShiftAssociation> ShiftAssociations => _shiftAssociations.AsReadOnly();

    private static readonly List<Staff> _extent = new();
    public static void ClearExtentForTests() => _extent.Clear();
    private static void AddToExtent(Staff manager)
    {
        if (manager == null)
            throw new ArgumentNullException(nameof(manager));
        
        if (_extent.Any(m => m.StaffId == manager.StaffId))
            throw new ArgumentException($"Manager with ID {manager.StaffId} already exists");
            
        _extent.Add(manager);
    }
    
    private readonly List<Staff> _managedStaff = new();
    public IReadOnlyCollection<Staff> ManagedStaff => _managedStaff.AsReadOnly();
    public void AddManagedStaff(Staff staff)
    {
        if (staff == null)
            throw new ArgumentNullException(nameof(staff));
        
        if (staff == this)
            throw new InvalidOperationException("Manager cannot manage themselves");
        
        if (_managedStaff.Contains(staff))
            throw new ArgumentException($"{staff.LastName} is already managed by {LastName}");
        
        if (staff is Staff managerStaff && managerStaff.Level > this.Level)
        {
            throw new InvalidOperationException(
                $"{LastName} (Level {Level}) cannot manage {managerStaff.LastName} (Level {managerStaff.Level})");
        }
        
        if (staff.Manager != null && staff.Manager != this)
        {
            staff.Manager.RemoveManagedStaffInternal(staff);
        }
        
        _managedStaff.Add(staff);
        staff.AssignManager(this); 
    }

    public bool RemoveManagedStaff(Staff staff)
    {
        if (staff == null || !_managedStaff.Contains(staff))
            return false;
        
        _managedStaff.Remove(staff);
        staff.RemoveManager();  
        return true;
    }

    internal void RemoveManagedStaffInternal(Staff staff)
    {
        _managedStaff.Remove(staff);
    }

    public static IReadOnlyCollection<Staff> GetExtent() => _extent.AsReadOnly();
    public int StaffId { get; }
    
    private string _firstName;
    public string FirstName 
    { 
        get => _firstName; 
        set 
        { 
            if (string.IsNullOrWhiteSpace(value)) 
                throw new ArgumentException("First name cannot be empty"); 
            _firstName = value.Trim(); 
        } 
    }
    
    private string _lastName;
    public string LastName 
    { 
        get => _lastName; 
        set 
        { 
            if (string.IsNullOrWhiteSpace(value)) 
                throw new ArgumentException("Last name cannot be empty"); 
            _lastName = value.Trim(); 
        } 
    }
    
    private decimal _salary;
    public decimal Salary 
    { 
        get => _salary; 
        set 
        { 
            if (value < 0) 
                throw new ArgumentException("Salary cannot be negative"); 
            _salary = value; 
        } 
    }
    
    private string _department;
    public string Department 
    { 
        get => _department; 
        set 
        { 
            if (string.IsNullOrWhiteSpace(value)) 
                throw new ArgumentException("Department cannot be empty"); 
            _department = value.Trim(); 
        } 
    }
    
    public string? Phone { get; set; }
    public string? Email { get; set; }
    
    [JsonInclude]
    private Staff _manager;
    public Staff Manager => _manager;
    
    private readonly List<Ingredient> _ingredients = new();
    public IReadOnlyCollection<Ingredient> Ingredients => _ingredients.AsReadOnly();

    public void AddIngredient(Ingredient ingredient)
    {
        if (ingredient == null)
            throw new ArgumentNullException(nameof(ingredient));

        if (!_ingredients.Contains(ingredient))
        {
            _ingredients.Add(ingredient);
            ingredient.AddChef(this); 
        }
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        if (ingredient == null)
            return;

        if (_ingredients.Remove(ingredient))
        {
            ingredient.RemoveChef(this); 
        }
    }

    
    public StaffType StaffType { get; private set; }
    
    public Level? Level { get; private set; }
    
    public bool? HasAccessToVault { get; private set; }
    
    public Table? Tables { get; set; }
    
    public string? SignatureDish { get; private set; }
    
    public Staff(int staffId, string firstName, string lastName, decimal salary, string department, StaffType staffType)
    {
        if (staffId <= 0) throw new ArgumentException("Staff ID can't be zero or negative");
        StaffId = staffId;
        FirstName = firstName;
        LastName = lastName;
        Salary = salary;
        Department = department;
        StaffType = staffType;
    }

    // ============================
    // FACTORY METHODS
    // ============================
    public static Staff CreateManager(int staffId, string firstName, string lastName, decimal salary, string department, Level level)
    {
        var staff = new Staff(staffId, firstName, lastName, salary, department, StaffType.Manager)
        {
            Level = level
        };
        staff.ValidateConditionalAttributes();
        return staff;
    }

    public static Staff CreateCashier(int staffId, string firstName, string lastName, decimal salary, string department, bool hasAccessToVault)
    {
        var staff = new Staff(staffId, firstName, lastName, salary, department, StaffType.Cashier)
        {
            HasAccessToVault = hasAccessToVault
        };
        staff.ValidateConditionalAttributes();
        return staff;
    }

    public static Staff CreateWaiter(int staffId, string firstName, string lastName, decimal salary, string department, Table tables)
    {
        var staff = new Staff(staffId, firstName, lastName, salary, department, StaffType.Waiter)
        {
            Tables = tables ?? throw new ArgumentNullException(nameof(tables), "Tables must be provided for Waiter")
        };
        staff.ValidateConditionalAttributes();
        return staff;
    }

    public static Staff CreateChef(int staffId, string firstName, string lastName, decimal salary, string department)
    {
        var staff = new Staff(staffId, firstName, lastName, salary, department, StaffType.Chef);
        staff.ValidateConditionalAttributes();
        return staff;
    }

    public static Staff CreateHeadChef(int staffId, string firstName, string lastName, decimal salary, string department, string signatureDish)
    {
        var staff = new Staff(staffId, firstName, lastName, salary, department, StaffType.HeadChef)
        {
            SignatureDish = signatureDish
        };
        staff.ValidateConditionalAttributes();
        return staff;
    }

    // ============================
    // VALIDATION
    // ============================
    public void ValidateConditionalAttributes()
    {
        switch (StaffType)
        {
            case StaffType.Manager:
                if (Level == null)
                    throw new InvalidOperationException("Level must be set for Manager.");
                if (HasAccessToVault != null || Tables != null || SignatureDish != null)
                    throw new InvalidOperationException("Non-manager properties must be null for Manager.");
                break;
                
            case StaffType.Cashier:
                if (HasAccessToVault == null)
                    throw new InvalidOperationException("HasAccessToVault must be set for Cashier.");
                if (Level != null || Tables != null || SignatureDish != null)
                    throw new InvalidOperationException("Non-cashier properties must be null for Cashier.");
                break;
                
            case StaffType.Waiter:
                if (Tables == null)
                    throw new InvalidOperationException("Tables must be set for Waiter.");
                if (Level != null || HasAccessToVault != null || SignatureDish != null)
                    throw new InvalidOperationException("Non-waiter properties must be null for Waiter.");
                break;
                
            case StaffType.Chef:
                if (Level != null || HasAccessToVault != null || Tables != null || SignatureDish != null)
                    throw new InvalidOperationException("Non-chef properties must be null for Chef.");
                break;
                
            case StaffType.HeadChef:
                if (SignatureDish == null)
                    throw new InvalidOperationException("SignatureDish must be set for HeadChef.");
                if (Level != null || HasAccessToVault != null || Tables != null)
                    throw new InvalidOperationException("Non-headchef properties must be null for HeadChef.");
                break;
                
            default:
                throw new InvalidOperationException("Unknown StaffType.");
        }
    }

    // ============================
    // TYPE-SPECIFIC METHODS
    // ============================
    public void HireStaff()
    {
        if (StaffType != StaffType.Manager)
            throw new InvalidOperationException("Only managers can hire staff.");
        Console.WriteLine($"Manager {LastName} is hiring staff.");
    }

    public void FireStaff()
    {
        if (StaffType != StaffType.Manager)
            throw new InvalidOperationException("Only managers can fire staff.");
        Console.WriteLine($"Manager {LastName} is firing staff.");
    }

    public void ManageEmployees()
    {
        if (StaffType != StaffType.Manager)
            throw new InvalidOperationException("Only managers can manage employees.");
        Console.WriteLine($"Manager {LastName} is managing employees.");
    }

    public void ChangeStaffShift()
    {
        if (StaffType != StaffType.Manager)
            throw new InvalidOperationException("Only managers can change staff shifts.");
        Console.WriteLine($"Manager {LastName} is changing staff shift.");
    }

    public void IssueRefund(decimal amount, string orderId, string reason, string authorizationCode)
    {
        
        if (StaffType != StaffType.Cashier)
            throw new InvalidOperationException("Only cashiers can issue refunds.");
        if (amount <= 0)
        throw new ArgumentException("Refund amount must be positive.", nameof(amount));
    if (string.IsNullOrWhiteSpace(orderId))
        throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
    if (string.IsNullOrWhiteSpace(reason))
        throw new ArgumentException("Refund reason cannot be empty.", nameof(reason));
    if (string.IsNullOrWhiteSpace(authorizationCode))
        throw new ArgumentException("Manager authorization required.", nameof(authorizationCode));
    
    
    Console.WriteLine($"[REFUND ISSUED] Order: {orderId}, Amount: {amount:C}, Reason: {reason}");
    Console.WriteLine($"[AUTHORIZATION] Code: {authorizationCode}, Processed by Cashier: {LastName}");
    }

    public void ReceivePayment(decimal amount, string paymentMethod, string orderId)
    {
        if (StaffType != StaffType.Cashier)
            throw new InvalidOperationException("Only cashiers can issue refunds.");
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be positive.", nameof(amount));
        if (string.IsNullOrWhiteSpace(paymentMethod))
            throw new ArgumentException("Payment method cannot be empty.", nameof(paymentMethod));
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
    
        string transactionId = $"TXN-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        Console.WriteLine($"[PAYMENT RECEIVED] Order: {orderId}, Amount: {amount:C}, Method: {paymentMethod}");
        Console.WriteLine($"[CONFIRMATION] Transaction ID: {transactionId}, Processed by Cashier: {LastName}");
    }

    public void SeatCustomer(Table table)
    {
        if (StaffType != StaffType.Waiter)
            throw new InvalidOperationException("Only waiters can issue refunds.");
        if (!table.IsOccupied)
        {
            Console.WriteLine("Customer seated");
        }
        else
        {
            Console.WriteLine("Table is ocupied");
        }
    }

    public void ManageInventory()
    {
        if (StaffType != StaffType.HeadChef)
            throw new InvalidOperationException("Only head chefs can manage inventory.");
        Console.WriteLine($"Head Chef {LastName} is managing inventory.");
    }

  
    internal void AssignManager(Staff manager) => _manager = manager ?? throw new ArgumentNullException(nameof(manager));
    internal void RemoveManager() => _manager = null;

    internal void AddHireDate(HireDate hireDate)
    {
        if (hireDate == null) throw new ArgumentNullException(nameof(hireDate));
        _hireDates.Add(hireDate);
    }
    
    internal void RemoveHireDate(HireDate hireDate) => _hireDates.Remove(hireDate);

    internal void AddShiftAssociation(ShiftAssociation association)
    {
        if (association == null) throw new ArgumentNullException(nameof(association));
        _shiftAssociations.Add(association);
    }
    
    internal void RemoveShiftAssociation(ShiftAssociation association) => _shiftAssociations.Remove(association);

    public HireDate? GetCurrentHireDate(Restaurant.Restaurant restaurant) => 
        _hireDates.FirstOrDefault(h => h.Restaurant == restaurant && h.IsCurrentlyEmployed);
        
    public ShiftAssociation? GetCurrentShiftAssociation(Restaurant.Restaurant restaurant) => 
        _shiftAssociations.FirstOrDefault(s => s.Restaurant == restaurant);
    
    public static void Save(string path = "staff.json")
    {
        try
        {
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve 
            };
            string jsonString = JsonSerializer.Serialize(_extent, options);
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save staffs.", ex);
        }
    }

    public static bool Load(string path = "staff.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _extent.Clear();
                return false;
            }
            
            string jsonString = File.ReadAllText(path);
            var loadedList = JsonSerializer.Deserialize<List<Staff>>(jsonString, 
                new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            
            _extent.Clear();
            if (loadedList != null)
            {
                _extent.AddRange(loadedList);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _extent.Clear();
            Console.WriteLine($"Failed to load managers: {ex.Message}");
            return false;
        }
    }
}