using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Main.Classes.Employees;
using Menu;            // ← THIS IS THE CORRECT NAMESPACE

namespace Main.Classes.Restaurant;

[Serializable]
public class Restaurant
{
    public int MaxCapacity { get; } = 100;
    public int RestaurantId { get; }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Restaurant name can't be null or empty");
            _name = value.Trim();
        }
    }

    private string _openingHour = string.Empty;
    public string OpeningHours
    {
        get => _openingHour;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Opening hours can't be null or empty");
            _openingHour = value.Trim();
        }
    }

    private readonly List<HireDate> _hireDates = new();
    public IReadOnlyCollection<HireDate> HireDates => _hireDates.AsReadOnly();

    private readonly List<ShiftAssociation> _shiftAssociations = new();
    public IReadOnlyCollection<ShiftAssociation> ShiftAssociations => _shiftAssociations.AsReadOnly();

    // ----- TABLES -----
    internal readonly List<Table> _tables = new();
    public IReadOnlyCollection<Table> Tables => _tables.AsReadOnly();

    // ----- MENUS (qualified by name) -----
    [JsonIgnore]
    private readonly Dictionary<string, global::Menu.Menu> _menus =
        new(StringComparer.OrdinalIgnoreCase);

    [JsonIgnore]
    public IReadOnlyCollection<global::Menu.Menu> Menus =>
        _menus.Values.ToList().AsReadOnly();

    private static readonly List<Restaurant> _extent = new();

    public Restaurant(int restaurantId, string name, string openingHours)
    {
        if (restaurantId <= 0)
            throw new ArgumentException("Restaurant id can't be zero or negative");

        RestaurantId = restaurantId;
        Name = name;
        OpeningHours = openingHours;

        AddToExtent(this);
    }

    private static void AddToExtent(Restaurant restaurant)
    {
        if (_extent.Any(r => r.RestaurantId == restaurant.RestaurantId))
            throw new ArgumentException($"Restaurant with ID {restaurant.RestaurantId} already exists");

        _extent.Add(restaurant);
    }

    public static IReadOnlyCollection<Restaurant> GetExtent() => _extent.AsReadOnly();
    public static void ClearExtentForTest() => _extent.Clear();

    // ----- MENU MANAGEMENT -----

    public global::Menu.Menu AddMenu(int menuId, string name, string version, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Menu name cannot be empty.");

        var key = name.Trim();

        if (_menus.ContainsKey(key))
            throw new ArgumentException($"Menu '{key}' already exists.");

        var menu = new global::Menu.Menu(menuId, key, version, isActive);
        _menus.Add(key, menu);

        return menu;
    }

    public global::Menu.Menu? GetMenuByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Menu name cannot be empty.");

        _menus.TryGetValue(name.Trim(), out var menu);
        return menu;
    }

    public bool RemoveMenu(string name)
    {
        var key = name.Trim();

        if (!_menus.ContainsKey(key))
            return false;

        if (_menus.Count == 1)
            throw new InvalidOperationException("Restaurant must have at least one menu.");

        var menu = _menus[key];
        _menus.Remove(key);

        global::Menu.Menu.RemoveFromExtent(menu);

        return true;
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

    //  SHIFT ASSOCIATION METHODS
    internal void AddShiftAssociation(ShiftAssociation association)
    {
        if (association == null) throw new ArgumentNullException(nameof(association));
        _shiftAssociations.Add(association);
    }
    
    internal void RemoveShiftAssociation(ShiftAssociation association)
    {
        _shiftAssociations.Remove(association);
    }


    // ----- TABLES -----

    public Table AddTable(int tableId, int number)
    {
        if (_tables.Any(t => t.TableId == tableId))
            throw new ArgumentException($"Table {tableId} already exists in restaurant {RestaurantId}");

        return new Table(tableId, number,this);
    }
    
    internal void AddTableToCollection(Table table)
    {
        if (table == null)
            throw new ArgumentNullException(nameof(table));

        // prevent duplicates in this restaurant
        if (_tables.Any(t => t.TableId == table.TableId))
            throw new ArgumentException(
                $"Table {table.TableId} already exists in restaurant {RestaurantId}");

        _tables.Add(table);
    }

    internal void RemoveTableFromCollection(Table table)
    {
        if (table == null)
            return;

        _tables.Remove(table);
    }

    // ----- DELETION -----

    public void DeleteRestaurant()
    {
        foreach (var menu in _menus.Values.ToList())
            global::Menu.Menu.RemoveFromExtent(menu);

        _menus.Clear();
        _tables.Clear();

        _extent.Remove(this);
    }

    // ----- SAVE / LOAD -----

    public static void Save(string path = "Restaurant.json")
    {
        var json = JsonSerializer.Serialize(_extent,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });

        File.WriteAllText(path, json);
    }

    public static bool Load(string path = "Restaurant.json")
    {
        if (!File.Exists(path))
        {
            _extent.Clear();
            return false;
        }

        var jsonString = File.ReadAllText(path);
        var loaded = JsonSerializer.Deserialize<List<Restaurant>>(jsonString);

        _extent.Clear();
        _extent.AddRange(loaded ?? new List<Restaurant>());

        return true;
    }
}