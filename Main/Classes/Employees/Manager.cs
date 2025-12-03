using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Main.Classes.Employees;

[Serializable]
public class Manager : Staff
{
    private static readonly List<Manager> _managerExtent = new();
    
    private readonly List<Staff> _managedStaff = new();
    public IReadOnlyCollection<Staff> ManagedStaff => _managedStaff.AsReadOnly();
    
    public ManagerLevels Level { get; set; }

    public Manager(int staffId, string firstName, string lastName, decimal salary, string department, ManagerLevels level)
        : base(staffId, firstName, lastName, salary, department)
    {
        Level = level;
        AddToExtent(this);
    }

    private static void AddToExtent(Manager manager)
    {
        if (manager == null)
            throw new ArgumentNullException(nameof(manager));
        
        if (_managerExtent.Any(m => m.StaffId == manager.StaffId))
            throw new ArgumentException($"Manager with ID {manager.StaffId} already exists");
            
        _managerExtent.Add(manager);
    }

    public static IReadOnlyCollection<Manager> GetExtent() => _managerExtent.AsReadOnly();
    public static void ClearExtentForTests() => _managerExtent.Clear();

    public void AddManagedStaff(Staff staff)
    {
        if (staff == null)
            throw new ArgumentNullException(nameof(staff));
        
        if (staff == this)
            throw new InvalidOperationException("Manager cannot manage themselves");
        
        if (_managedStaff.Contains(staff))
            throw new ArgumentException($"{staff.LastName} is already managed by {LastName}");
        
        if (staff is Manager managerStaff && managerStaff.Level > this.Level)
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

    public override void hireStaff()
    {
        Console.WriteLine($"Manager {LastName} is hiring staff.");
    }

    public override void fireStaff()
    {
        Console.WriteLine($"Manager {LastName} is firing staff.");
    }

    public void ManageEmployee()
    {
        Console.WriteLine($"Manager {LastName} is managing employees.");
    }

    public void ChangeStaffShift()
    {
        Console.WriteLine($"Manager {LastName} is changing staff shift.");
    }

    public static void Save(string path = "managers.json")
    {
        try
        {
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve 
            };
            string jsonString = JsonSerializer.Serialize(_managerExtent, options);
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save managers.", ex);
        }
    }

    public static bool Load(string path = "managers.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _managerExtent.Clear();
                return false;
            }
            
            string jsonString = File.ReadAllText(path);
            var loadedList = JsonSerializer.Deserialize<List<Manager>>(jsonString, 
                new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            
            _managerExtent.Clear();
            if (loadedList != null)
            {
                _managerExtent.AddRange(loadedList);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _managerExtent.Clear();
            Console.WriteLine($"Failed to load managers: {ex.Message}");
            return false;
        }
    }
}