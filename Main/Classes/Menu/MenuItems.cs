namespace Menu;

[Serializable]
public abstract class MenuItems
{
    public Guid ItemId { get; }
    private string _name;
    private decimal _price;
    private bool _isAvailable;
    private string? _description;


    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty");
            _name = value.Trim();
        }
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
                throw new ArgumentException("Price cannot be negative");
            _price = Math.Round(value, 2);
        }
    }

    public bool IsAvailable
    {
        get => _isAvailable;
        set => _isAvailable = value;
    }

    public string? Description
    {
        get => _description;
        set
        {
            if (value is not null && string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Description cannot be blank");
            _description = value;
        }
    }

    private List<string> _allergens = new();
    public IReadOnlyList<string> Allergens => _allergens.AsReadOnly();

    public void AddAllergen(string allergen)
    {
        if (string.IsNullOrWhiteSpace(allergen))
            throw new ArgumentException("Allergen cannot be empty");
        if (!_allergens.Contains(allergen.Trim()))
            _allergens.Add(allergen.Trim());
    }

    public void RemoveAllergen(string allergen) => _allergens.Remove(allergen);

    protected MenuItems(string name, decimal price, bool isAvailable, string? description = null)
    {
        ItemId = Guid.NewGuid();
        Name = name;
        Price = price;
        IsAvailable = isAvailable;
        Description = description;
    }
    

    public static void CreateMenuItem(MenuItems item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
    }

    public virtual void UpdateMenuItem(string? name = null, decimal? price = null,
                                       bool? isAvailable = null, string? description = null)
    {
        if (name is not null) Name = name;
        if (price is not null) Price = price.Value;
        if (isAvailable is not null) IsAvailable = isAvailable.Value;
        if (description is not null) Description = description;
    }
}
