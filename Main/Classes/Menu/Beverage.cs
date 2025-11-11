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
}