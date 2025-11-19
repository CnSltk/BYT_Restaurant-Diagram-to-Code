using System.Text.Json;

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
    private string _name = string.Empty;
    private int _timeUsed;

    private static List<Ingredient> _extent = new();

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

    private readonly List<string> _allergens = new();
    public IReadOnlyList<string> Allergens => _allergens.AsReadOnly();

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

    public Ingredient(string name, Unit unit, IEnumerable<string>? allergens = null)
    {
        _name = name;
        Unit = unit;

        if (allergens != null)
        {
            foreach (var a in allergens)
            {
                if (!string.IsNullOrWhiteSpace(a))
                    AddAllergen(a);
            }
        }

        AddToExtent(this);
    }

    public void AddAllergen(string allergen)
    {
        if (string.IsNullOrWhiteSpace(allergen))
            throw new ArgumentException("Allergen cannot be empty");

        var trimmed = allergen.Trim();
        if (trimmed.Length > 50)
            throw new ArgumentException("Allergen name is too long");
        if (_allergens.Contains(trimmed))
            throw new ArgumentException("Allergen already exists");
        if (_allergens.Count >= 10)
            throw new ArgumentException("Too many allergens");

        _allergens.Add(trimmed);
    }

    public void RemoveAllergen(string allergen)
    {
        _allergens.Remove(allergen);
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
