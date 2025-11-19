using System.Text.Json;

namespace Menu;

[Serializable]
public class Menu
{
    private string _name;
    public Guid MenuId { get; }
    public string Version { get; set; }
    public bool IsActive { get; set; }

    private static List<Menu> _extent = new List<Menu>();

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Menu name cannot be empty");
            _name = value.Trim();
        }
    }

    public Menu(string name, string version, bool isActive)
    {
        MenuId = Guid.NewGuid();
        Name = name;
        Version = version;
        IsActive = isActive;
        AddToExtent(this);
    }

    private static void AddToExtent(Menu menu)
    {
        if (menu == null)
            throw new ArgumentNullException(nameof(menu));
        _extent.Add(menu);
    }

    public static IReadOnlyList<Menu> GetExtent() => _extent.AsReadOnly();

    public static void ClearExtentForTest() => _extent.Clear();
    
    
    public static void Save(string path = "Menu.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_extent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save Menu.", ex);
        }
    }

    public static bool Load(string path = "Menu.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _extent.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            _extent = JsonSerializer.Deserialize<List<Menu>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            _extent.Clear();
            return false;
        }
    }
}