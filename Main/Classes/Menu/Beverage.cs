using System.Text.Json;
using Main.Classes.Employees;

namespace Menu;

public enum BeverageCategory
{
    Alcoholic,
    SoftBeverage,
    HotBeverage
}

[Serializable]
public class Beverage : MenuItems
{
    private int _volumeMl;

    public int VolumeMl
    {
        get => _volumeMl;
        set
        {
            if (value != 500 && value != 1000)
                throw new ArgumentException("Volume must be 500 ml or 1000 ml");
            _volumeMl = value;
        }
    }

    public BeverageCategory Category { get; set; }
    
    private static List<Beverage> _extent = new List<Beverage>();
    
    private static void AddToExtent(Beverage beverage)
    {
        if (beverage == null)
            throw new ArgumentException("Beverage cannot be null.");
        _extent.Add(beverage);
    }

    public static IReadOnlyList<Beverage> GetExtent()
    {
        return _extent.AsReadOnly();
    }
    
    

    public Beverage(
        string name,
        decimal price,
        bool isAvailable,
        int volumeMl,
        BeverageCategory category,
        string? description = null
    ) : base(name, price, isAvailable, description)
    {
        VolumeMl = volumeMl;
        Category = category;
    }
    
    public static void Save(string path = "Beverage.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_extent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save Beverage.", ex);
        }
    }

    public static bool Load(string path = "Beverage.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _extent.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            _extent = JsonSerializer.Deserialize<List<Beverage>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            _extent.Clear();
            return false;
        }
    }
}