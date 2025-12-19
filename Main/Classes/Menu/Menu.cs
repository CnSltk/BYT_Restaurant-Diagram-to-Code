using System.Text.Json;
using Main.Classes.Restaurant;

namespace Menu;
 
[Serializable]
public class Menu
{
    private string _name = string.Empty;
    private string _version = string.Empty;
    private Restaurant _restaurant;
 
    public int MenuId { get; }
 
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Menu name cannot be empty");
 
            var trimmed = value.Trim();
            if (trimmed.Length < 2 || trimmed.Length > 50)
                throw new ArgumentException("Menu name length must be between 2 and 50 characters");
 
            _name = trimmed;
        }
    }
 
    public string Version
    {
        get => _version;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Version cannot be empty");
 
            var trimmed = value.Trim();
            if (trimmed.Length > 20)
                throw new ArgumentException("Version is too long");
 
            _version = trimmed;
        }
    }
 
    public bool IsActive { get; set; }
 
    private static List<Menu> _extent = new();
 
    private List<MenuItems> _items = new();
    public IReadOnlyList<MenuItems> Items => _items.AsReadOnly();
 
    public Menu(int menuId, string name, string version, bool isActive, Restaurant restaurant)
    {
        MenuId = menuId;
        Name = name;
        Version = version;
        IsActive = isActive;
        _restaurant = restaurant;
 
        AddToExtent(this);
    }
 
    public void AddMenuItem(MenuItems item)
    {
        if (item == null)
            throw new ArgumentException("MenuItem cannot be null.");
 
        if (_items.Contains(item))
            throw new InvalidOperationException("MenuItem already added to this Menu.");
 
        _items.Add(item);
 
        if (!item.Menus.Contains(this))
            item.AddMenuInternal(this);
    }
 
    public void RemoveMenuItem(MenuItems item)
    {
        if (!_items.Contains(item))
            throw new InvalidOperationException("MenuItem not in this Menu.");
 
        if (_items.Count == 1)
            throw new InvalidOperationException("Menu must contain at least one MenuItem.");
 
        _items.Remove(item);
 
        if (item.Menus.Contains(this))
            item.RemoveMenuInternal(this);
    }
 
    private static void AddToExtent(Menu menu)
    {
        if (menu != null) _extent.Add(menu);
    }
 
    public static void RemoveFromExtent(Menu menu)
    {
        if (menu != null) _extent.Remove(menu);
    }
 
    public static IReadOnlyList<Menu> GetExtent() => _extent.AsReadOnly();
 
    public static void ClearExtentForTest() => _extent.Clear();
 
    public static void Save(string path = "Menu.json")
    {
        var jsonString = JsonSerializer.Serialize(_extent, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, jsonString);
    }
 
    public static bool Load(string path = "Menu.json")
    {
        if (!File.Exists(path))
        {
            _extent.Clear();
            return false;
        }
 
        var jsonString = File.ReadAllText(path);
        var loaded = JsonSerializer.Deserialize<List<Menu>>(jsonString);
        _extent = loaded ?? new List<Menu>();
 
        return true;
    }
}