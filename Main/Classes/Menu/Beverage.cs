using System.Text.Json;
using Main.Classes.Menu;

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

    private static List<Beverage> _extent = new();
    public static IReadOnlyList<Beverage> GetExtent() => _extent.AsReadOnly();
    public static void ClearExtentForTests() => _extent.Clear();

    private static void AddToExtent(Beverage beverage)
    {
        if (beverage == null)
            throw new ArgumentNullException(nameof(beverage));
        _extent.Add(beverage);
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
        AddToExtent(this);
    }

    public static void Save(string path = "Beverage.json")
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(_extent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save Beverage extent.", ex);
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

            var jsonString = File.ReadAllText(path);
            var loaded = JsonSerializer.Deserialize<List<Beverage>>(jsonString);
            _extent = loaded ?? new List<Beverage>();
            return true;
        }
        catch
        {
            _extent.Clear();
            return false;
        }
    }
}
