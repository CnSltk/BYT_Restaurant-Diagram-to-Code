using System.Text.Json;
using Menu;

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
    public int IngredientId { get; }
    private string _name;

    public string Name
    {
        get => _name;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Ingredient name can't be empty");
            _name = value.Trim();
        }
    }
    public Unit Unit { get; private set; }

    public List<string> Allergens { get; } = new List<string>();
    public int TimeUsed { get; set; }
    private static List<Ingredient> _extent = new List<Ingredient>();
    
    
    
    public Ingredient(int ingredientId, string name,Unit unit,IEnumerable<string> allergens = null)
        {
        if (ingredientId <= 0)
            throw new ArgumentException("Ingredient id can't be zero or negative");
        IngredientId = ingredientId;
        Name = name;
        Unit = unit;
        if (allergens != null)
        {
            foreach (var a in allergens)
            {
                if(!string.IsNullOrWhiteSpace(a))
                    Allergens.Add(a.Trim());
            }
        }

        AddToExtent(this);
        }

    private static void AddToExtent(Ingredient ingredient)
    {
        if(ingredient == null)
            throw new ArgumentNullException(nameof(ingredient));
        _extent.Add(ingredient);
    }
    public static IReadOnlyList<Ingredient> GetExtent()
        => _extent.AsReadOnly();
    public static void ClearExtentForTests()
        => _extent.Clear();
    
    
    public static void Save(string path = "Ingredient.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_extent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save Ingredient.", ex);
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
            string jsonString = File.ReadAllText(path);
            _extent = JsonSerializer.Deserialize<List<Ingredient>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            _extent.Clear();
            return false;
        }
    }

    public void UseIngredient()
    {
        TimeUsed++;
    }
}