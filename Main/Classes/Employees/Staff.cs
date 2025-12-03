using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Main.Classes.Employees;

[Serializable]
public abstract class Staff
{
    public int StaffId { get; }
    
    private string _firstName; public String FirstName { get => _firstName; set { if(string.IsNullOrEmpty(value)) throw new ArgumentException("First name cannot be empty"); _firstName = value; } }
    private string _lastName; public String LastName { get => _lastName; set { if(string.IsNullOrEmpty(value)) throw new ArgumentException("Last name cannot be empty"); _lastName = value; } }
    private decimal _salary; public Decimal Salary { get => _salary; set { if(value < 0) throw new ArgumentException("Salary cannot be negative"); _salary = value; } }
    private string _department; public String Department { get => _department; set { if(string.IsNullOrEmpty(value)) throw new ArgumentException("Department cannot be empty"); _department = value; } }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    [JsonInclude] private Manager _manager; public Manager Manager => _manager;
    
    // ✅ BOTH COLLECTIONS
    private readonly List<HireDate> _hireDates = new();
    public IReadOnlyCollection<HireDate> HireDates => _hireDates.AsReadOnly();
    
    private readonly List<ShiftAssociation> _shiftAssociations = new();
    public IReadOnlyCollection<ShiftAssociation> ShiftAssociations => _shiftAssociations.AsReadOnly();
    
    protected Staff(int staffId, string firstName, string lastName, decimal salary, string department)
    {
        if(staffId <= 0) throw new ArgumentException("Staff ID can't be zero or negative");
        StaffId = staffId; FirstName = firstName; LastName = lastName; Salary = salary; Department = department; Phone = null; Email = null;
    }
    
    public abstract void hireStaff();
    public abstract void fireStaff();
    
    internal void AssignManager(Manager manager) => _manager = manager;
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

    public HireDate? GetCurrentHireDate(Restaurant.Restaurant restaurant) => _hireDates.FirstOrDefault(h => h.Restaurant == restaurant && h.IsCurrentlyEmployed);
    public ShiftAssociation? GetCurrentShiftAssociation(Restaurant.Restaurant restaurant) => _shiftAssociations.FirstOrDefault(s => s.Restaurant == restaurant);
}