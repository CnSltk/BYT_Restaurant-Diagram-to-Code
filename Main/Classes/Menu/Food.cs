using System.Text.Json;

namespace Menu;

public enum FoodCategory
{
    Starter,
    MainCourse,
    Dessert
}

[Serializable]
public class Food : MenuItems
{
    public bool SpiceLevel { get; set; }
    public bool IsVegetarian { get; set; }

    private int _calories;
    private int _prepTimeMin;

    public int Calories
    {
        get => _calories;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Calories must be greater than zero");
            if (value > 5000)
                throw new ArgumentException("Calories value is too high");
            _calories = value;
        }
    }

    public int PrepTimeMin
    {
        get => _prepTimeMin;
        set
        {
            if (value < 0)
                throw new ArgumentException("Preparation time cannot be negative");
            if (value > 240)
                throw new ArgumentException("Preparation time is too long");
            _prepTimeMin = value;
        }
    }

    public FoodCategory Category { get; set; }

    private static List<Food> _extent = new();

    private static void AddToExtent(Food food)
    {
        if (food == null)
            throw new ArgumentNullException(nameof(food));
        _extent.Add(food);
    }

    public static IReadOnlyList<Food> GetExtent() => _extent.AsReadOnly();

    public static void ClearExtentForTests() => _extent.Clear();

    public static void Save(string path = "Food.json")
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(_extent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save Food extent.", ex);
        }
    }

    public static bool Load(string path = "Food.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _extent.Clear();
                return false;
            }

            var jsonString = File.ReadAllText(path);
            var loaded = JsonSerializer.Deserialize<List<Food>>(jsonString);
            _extent = loaded ?? new List<Food>();
            return true;
        }
        catch
        {
            _extent.Clear();
            return false;
        }
    }

    public Food(
        string name,
        decimal price,
        bool isAvailable,
        bool spiceLevel,
        bool isVegetarian,
        int calories,
        int prepTimeMin,
        FoodCategory category,
        string? description = null
    ) : base(name, price, isAvailable, description)
    {
        SpiceLevel = spiceLevel;
        IsVegetarian = isVegetarian;
        Calories = calories;
        PrepTimeMin = prepTimeMin;
        Category = category;
        AddToExtent(this);
    }
}
