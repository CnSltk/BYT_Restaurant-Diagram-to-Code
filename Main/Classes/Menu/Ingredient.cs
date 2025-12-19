using System.Text.Json;
using Main.Classes.Employees;
using System.Collections.Generic;
 
namespace Menu;
 
public enum Unit
{
    Gram,
    Ml,
    Pcs
}
 
[Serializable]
public class Ingredient
{
    private int _timeUsed;
    public int TimeUsed
    {
        get => _timeUsed;
        set
        {
            if (value < 0)
                throw new ArgumentException("TimeUsed cannot be negative");
            _timeUsed = value;
        }
    }
 
    public int IngredientId { get; private set; }
 
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Ingredient name can't be empty");
 
            var trimmed = value.Trim();
            if (trimmed.Length < 2 || trimmed.Length > 50)
                throw new ArgumentException("Ingredient name length must be between 2 and 50 characters");
 
            _name = trimmed;
        }
    }
 
    public Unit Unit { get; private set; }
 
    private readonly List<Staff> _chefs = new();
    public IReadOnlyCollection<Staff> Chefs => _chefs.AsReadOnly();
 
    internal void AddChef(Staff chef)
    {
        if (!_chefs.Contains(chef))
            _chefs.Add(chef);
    }
 
    internal void RemoveChef(Staff chef)
    {
        _chefs.Remove(chef);
    }
 
    private static List<Ingredient> _extent = new();
 
    private List<MenuItems> _menuItems = new();
    public IReadOnlyList<MenuItems> MenuItems => _menuItems.AsReadOnly();
 
    internal void AddMenuItemInternal(MenuItems item)
    {
        if (!_menuItems.Contains(item))
            _menuItems.Add(item);
    }
 
    public void AddMenuItem(MenuItems item)
    {
        if (item == null)
            throw new ArgumentException("MenuItem cannot be null.");
 
        if (_menuItems.Contains(item))
            throw new InvalidOperationException("MenuItem already linked to this Ingredient.");
 
        _menuItems.Add(item);
 
        if (!item.Ingredients.Contains(this))
            item.AddIngredient(this);
    }
 
    public Ingredient(int ingredientId, string name, Unit unit)
    {
        IngredientId = ingredientId;
        Name = name;
        Unit = unit;
 
        AddToExtent(this);
    }
 
    public void UseIngredient()
    {
        if (TimeUsed == int.MaxValue)
            throw new InvalidOperationException("TimeUsed reached maximum value");
        TimeUsed++;
    }
 
    private static void AddToExtent(Ingredient ingredient)
    {
        if (ingredient == null)
            throw new ArgumentNullException(nameof(ingredient));
        _extent.Add(ingredient);
    }
 
    public static IReadOnlyList<Ingredient> GetExtent() => _extent.AsReadOnly();
 
    public static void ClearExtentForTests() => _extent.Clear();
 
    public static void Save(string path = "Ingredient.json")
    {
        var jsonString = JsonSerializer.Serialize(_extent, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, jsonString);
    }
 
    public static bool Load(string path = "Ingredient.json")
    {
        if (!File.Exists(path))
        {
            _extent.Clear();
            return false;
        }
 
        var jsonString = File.ReadAllText(path);
        var loaded = JsonSerializer.Deserialize<List<Ingredient>>(jsonString);
        _extent = loaded ?? new List<Ingredient>();
        return true;
    }
}



