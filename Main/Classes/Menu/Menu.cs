using System.Text.Json;

namespace Menu;

[Serializable]
public class Menu
{
    private string _name = string.Empty;
    private string _version = string.Empty;

    public Guid MenuId { get; }

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

    public Menu(string name, string version, bool isActive)
    {
        MenuId = Guid.NewGuid();
        _name = name ?? string.Empty;
        _version = version ?? string.Empty;
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
            var jsonString = JsonSerializer.Serialize(_extent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save Menu extent.", ex);
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

            var jsonString = File.ReadAllText(path);
            var loaded = JsonSerializer.Deserialize<List<Menu>>(jsonString);
            _extent = loaded ?? new List<Menu>();
            return true;
        }
        catch
        {
            _extent.Clear();
            return false;
        }
    }
}
