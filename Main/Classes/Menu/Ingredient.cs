namespace Main.Classes.Menu;

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

    private string _unit;

    public string Unit
    {
        get => _unit;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Unit can't be empty");
            _unit = value.Trim();
        }
    }

    public List<string> Allergens { get; } = new List<string>();
    public int TimeUsed { get; private set; }
    private static List<Ingredient> _extent = new List<Ingredient>();
    
    public Ingredient(int ingredientId, string name, string unit,IEnumerable<string> allergens = null)
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