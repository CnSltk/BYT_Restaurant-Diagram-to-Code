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

    public void UseIngredient()
    {
        TimeUsed++;
    }
}