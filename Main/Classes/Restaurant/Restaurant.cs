using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Main.Classes.Employees;

namespace Main.Classes.Restaurant;

[Serializable]
public class Restaurant
{
    public int MaxCapacity { get; } = 100;
    public int RestaurantId { get; }

    private string _name; public string Name { get => _name; set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Restaurant name can't be null or empty"); _name = value.Trim(); } }
    private string _openingHour; public string OpeningHours { get => _openingHour; set { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Opening hours can't be null or empty"); _openingHour = value.Trim(); } }

    internal readonly List<Table> _tables = new();
    public IReadOnlyCollection<Table> Tables => _tables.AsReadOnly();

    private readonly List<HireDate> _hireDates = new();
    public IReadOnlyCollection<HireDate> HireDates => _hireDates.AsReadOnly();

    private readonly List<ShiftAssociation> _shiftAssociations = new();
    public IReadOnlyCollection<ShiftAssociation> ShiftAssociations => _shiftAssociations.AsReadOnly();

    private static readonly List<Restaurant> _extent = new();

    public Restaurant(int restaurantId, string name, string openingHours)
    {
        if (restaurantId <= 0) throw new ArgumentException("Restaurant id can't be zero or negative");
        RestaurantId = restaurantId; Name = name; OpeningHours = openingHours; AddToExtent(this);
    }
    
    private static void AddToExtent(Restaurant restaurant)
    {
        if (restaurant == null) throw new ArgumentNullException(nameof(restaurant));
        if (_extent.Any(r => r.RestaurantId == restaurant.RestaurantId)) throw new ArgumentException($"Restaurant with ID {restaurant.RestaurantId} already exists");
        _extent.Add(restaurant);
    }
    
    public static IReadOnlyCollection<Restaurant> GetExtent() => _extent.AsReadOnly();
    public static void ClearExtentForTest() => _extent.Clear();

    internal void AddTableToCollection(Table table)
    {
        if (table == null) throw new ArgumentNullException(nameof(table));
        if (_tables.Any(t => t.TableId == table.TableId)) throw new ArgumentException($"Table {table.TableId} already exists in restaurant");
        _tables.Add(table);
    }
    
    internal void RemoveTableFromCollection(Table table) => _tables.Remove(table);

    public Table AddTable(int tableId, int number)
    {
        if (_tables.Any(t => t.TableId == tableId)) throw new ArgumentException($"Table {tableId} already exists in restaurant {RestaurantId}");
        return new Table(tableId, number, this);
    }
    
    public bool RemoveTable(Table table)
    {
        if (table == null || !_tables.Contains(table)) return false;
        _tables.Remove(table); Table.RemoveFromExtent(table); return true;
    }

    public HireDate HireStaff(Staff staff, DateTime startDate)
    {
        if (staff.GetCurrentHireDate(this) != null)
            throw new InvalidOperationException($"{staff.FirstName} {staff.LastName} is already actively employed at {Name}");
        return HireDate.Create(staff, this, startDate);
    }

    public void TerminateStaff(Staff staff) => staff.GetCurrentHireDate(this)?.Terminate();
    public bool IsStaffCurrentlyEmployed(Staff staff) => staff.GetCurrentHireDate(this) != null;

    public ShiftAssociation AssignShift(Staff staff, ShiftType shiftType)
    {
        return ShiftAssociation.Create(staff, this, shiftType);
    }

    public void ChangeStaffShift(Staff staff, ShiftType newShift)
    {
        var association = staff.GetCurrentShiftAssociation(this);
        if (association == null) throw new InvalidOperationException("No shift assigned to this staff");
        association.ChangeShift(newShift);
    }

    public void DeleteRestaurant()
    {
        var hireDatesCopy = _hireDates.ToList();
        foreach (var hireDate in hireDatesCopy) hireDate.Remove();
        _hireDates.Clear();

        var shiftAssocCopy = _shiftAssociations.ToList();
        foreach (var association in shiftAssocCopy) association.Remove();
        _shiftAssociations.Clear();

        var tablesCopy = _tables.ToList();
        foreach (var table in tablesCopy) Table.RemoveFromExtent(table);
        _tables.Clear(); _extent.Remove(this);
    }

    internal void AddHireDate(HireDate hireDate)
    {
        if (hireDate == null) throw new ArgumentNullException(nameof(hireDate));
        _hireDates.Add(hireDate);
    }
    
    internal void RemoveHireDate(HireDate hireDate)
    {
        _hireDates.Remove(hireDate);
    }

    internal void AddShiftAssociation(ShiftAssociation association)
    {
        if (association == null) throw new ArgumentNullException(nameof(association));
        _shiftAssociations.Add(association);
    }
    
    internal void RemoveShiftAssociation(ShiftAssociation association)
    {
        _shiftAssociations.Remove(association);
    }

    public static void Save(string path = "Restaurant.json")
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve };
            File.WriteAllText(path, JsonSerializer.Serialize(_extent, options));
        }
        catch (Exception ex) { throw new InvalidOperationException("Failed to save Restaurant.", ex); }
    }

    public static bool Load(string path = "Restaurant.json")
    {
        try
        {
            if (!File.Exists(path)) { _extent.Clear(); Table.ClearExtentForTests(); return false; }
            var loadedList = JsonSerializer.Deserialize<List<Restaurant>>(File.ReadAllText(path));
            _extent.Clear(); _extent.AddRange(loadedList); 
            Table.ClearExtentForTests();
            foreach (var restaurant in _extent)
                foreach (var table in restaurant._tables) Table.AddToExtent(table);
            return true;
        }
        catch { _extent.Clear(); Table.ClearExtentForTests(); return false; }
    }
}