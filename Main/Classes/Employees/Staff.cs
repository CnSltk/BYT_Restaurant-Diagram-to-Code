using System;
using System.Text.Json.Serialization;

namespace Main.Classes.Employees;

[Serializable]
public abstract class Staff
{
    public int StaffId { get; }  
    
    private string _firstName;
    public String FirstName
    {
        get => _firstName;
        set
        {
            if(string.IsNullOrEmpty(value)) 
                throw new ArgumentException("First name cannot be empty");
            _firstName = value;
        }
    }
    
    private string _lastName;
    public String LastName
    {
        get => _lastName;
        set
        {
            if(string.IsNullOrEmpty(value))
                throw new ArgumentException("Last name cannot be empty");
            _lastName = value;
        }
    }
    
    private decimal _salary;
    public Decimal Salary
    {
        get => _salary;
        set
        {
            if(value < 0) 
                throw new ArgumentException("Salary cannot be negative");
            _salary = value;
        }
    }
    
    private string _department;
    public String Department
    {
        get => _department;
        set
        {
            if(string.IsNullOrEmpty(value)) 
                throw new ArgumentException("Department cannot be empty");
            _department = value;
        }
    }
    
    public string? Phone { get; set; }
    public string? Email { get; set; }
    
    [JsonInclude]  
    private Manager _manager;
    public Manager Manager => _manager;
    
    protected Staff(int staffId, string firstName, string lastName, decimal salary, string department)
    {
        if(staffId <= 0)
            throw new ArgumentException("Staff ID can't be zero or negative");
        StaffId = staffId;
        FirstName = firstName;
        LastName = lastName;
        Salary = salary;
        Department = department;
        Phone = null;
        Email = null;
    }
    
    public abstract void hireStaff();
    public abstract void fireStaff();
    
    internal void AssignManager(Manager manager)
    {
        _manager = manager;
    }
    
    internal void RemoveManager()
    {
        _manager = null;
    }
}