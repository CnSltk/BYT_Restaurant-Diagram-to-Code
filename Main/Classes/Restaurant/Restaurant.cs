using System.Text.Json;
using Main.Classes.Employees;

namespace Main.Classes.Restaurant;

[Serializable]
public class Restaurant
{
    public int MaxCapacity { get; } = 100;
    public int RestaurantId { get; }
    private string _name;

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
    private string _openingHour;
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
    private static List<Restaurant> _Restaurantextent = new List<Restaurant>();
    public Restaurant (int restaurantId, string name,string openingHours)
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
        if (restaurant == null)
            throw new ArgumentNullException(nameof(restaurant));
        _Restaurantextent.Add(restaurant);
    }
    public static IReadOnlyList<Restaurant> GetExtent() => _Restaurantextent.AsReadOnly();
    public static void ClearExtentForTest() => _Restaurantextent.Clear();
    
    public static void Save(string path = "Restaurant.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_Restaurantextent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save PartTime.", ex);
        }
    }

    public static bool Load(string path = "partTimes.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _Restaurantextent.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            _Restaurantextent = JsonSerializer.Deserialize<List<Restaurant>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            _Restaurantextent.Clear();
            return false;
        }
    }

}