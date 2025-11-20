using System.Text.Json;
using System.Text.Json.Serialization;

namespace Menu;

[Serializable]
public abstract class MenuItems
{
    private static List<MenuItems> _extent = new();
    public static IReadOnlyList<MenuItems> Extent => _extent.AsReadOnly();

    private static int _nextId = 1;

    public int ItemId { get; }

    private string _name = string.Empty;
    private decimal _price;
    private bool _isAvailable;
    private string? _description;

    private List<string> _allergens = new();
    public IReadOnlyList<string> Allergens => _allergens.AsReadOnly();

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty");
            if (value.Length < 2 || value.Length > 50)
                throw new ArgumentException("Name length must be between 2 and 50 characters");
            _name = value.Trim();
        }
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0m || value > 1000m)
                throw new ArgumentException("Price must be between 0 and 1000");
            _price = Math.Round(value, 2);
        }
    }

    public bool IsAvailable
    {
        get => _isAvailable;
        set => _isAvailable = value;
    }

    public string? Description
    {
        get => _description;
        set
        {
            if (value is not null)
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Description cannot be blank");
                if (value.Length > 200)
                    throw new ArgumentException("Description is too long");
                _description = value.Trim();
            }
            else
            {
                _description = null;
            }
        }
    }

    
    protected MenuItems(string name, decimal price, bool isAvailable, string? description = null)
    {
        ItemId = _nextId++;

        _name = name;
        _price = price;
        _isAvailable = isAvailable;
        _description = description;
    }

    
    public void AddAllergen(string allergen)
    {
        if (string.IsNullOrWhiteSpace(allergen))
            throw new ArgumentException("Allergen cannot be empty");
        allergen = allergen.Trim();
        if (allergen.Length > 50)
            throw new ArgumentException("Allergen name is too long");
        if (_allergens.Contains(allergen))
            throw new ArgumentException("Allergen already exists");
        if (_allergens.Count >= 10)
            throw new ArgumentException("Too many allergens");
        _allergens.Add(allergen);
    }

    public void RemoveAllergen(string allergen)
    {
        _allergens.Remove(allergen);
    }

    
    private static void AddToExtent(MenuItems item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        _extent.Add(item);
    }

    public static void CreateMenuItem(MenuItems item)
    {
        AddToExtent(item);
    }

    public static void ClearExtent() => _extent.Clear();

    
    public virtual void UpdateMenuItem(string? name = null, decimal? price = null,
                                       bool? isAvailable = null, string? description = null)
    {
        if (name is not null) Name = name;
        if (price is not null) Price = price.Value;
        if (isAvailable is not null) IsAvailable = isAvailable.Value;
        if (description is not null) Description = description;
    }

    
    public static void SaveExtent(string path)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        };

        File.WriteAllText(path, JsonSerializer.Serialize(_extent, options));
    }

    public static void LoadExtent(string path)
    {
        if (!File.Exists(path))
        {
            _extent = new List<MenuItems>();
            return;
        }

        var options = new JsonSerializerOptions
        {
            IncludeFields = true
        };

        var json = File.ReadAllText(path);
        var loaded = JsonSerializer.Deserialize<List<MenuItems>>(json, options);

        _extent = loaded ?? new List<MenuItems>();
    }
}
