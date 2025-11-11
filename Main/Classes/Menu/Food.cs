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
                throw new ArgumentException("Calories cannot be negative or zero");
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
            _prepTimeMin = value;
        }
    }

    public FoodCategory Category { get; set; }

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
    }
}