using System.Text.Json;
using Main.Classes.Employees; // Only if needed elsewhere
using System.Collections.Generic;

namespace Main.Classes.Menu;

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

    // ----- RELATION WITH CHEF -----
    private readonly List<Chef> _chefs = new();
    public IReadOnlyCollection<Chef> Chefs => _chefs.AsReadOnly();

    internal void AddChef(Chef chef)
    {
        if (!_chefs.Contains(chef))
            _chefs.Add(chef);
    }

    internal void RemoveChef(Chef chef)
    {
        _chefs.Remove(chef);
    }

    // ----- EXTENT -----
    private static List<Ingredient> _extent = new();

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
        try
        {
            var jsonString = JsonSerializer.Serialize(_extent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save Ingredient extent.", ex);
        }
    }

    public static bool Load(string path = "Ingredient.json")
    {
        try
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
        catch
        {
            _extent.Clear();
            return false;
        }
    }
}
