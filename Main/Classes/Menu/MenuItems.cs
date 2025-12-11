using System.Text.Json;
using System.Text.Json.Serialization;
using Main.Classes.Orders;
 
namespace Menu;
 
[Serializable]
public abstract class MenuItems
{
    private static List<MenuItems> _extent = new();
    public static IReadOnlyList<MenuItems> Extent => _extent.AsReadOnly();
 
    private static int _nextId = 1;
 
    public int ItemId { get; }
 
    private string _name = string.Empty;
    private decimal _price;
    private bool _isAvailable;
    private string? _description;
 
    public List<string> Allergens { get; set; } = new();
 
    // ============================
    // QUANTITY ASSOCIATION
    // ============================
 
    private List<Quantity> _quantities = new();
    public IReadOnlyList<Quantity> Quantities => _quantities.AsReadOnly();
 
    internal void AddQuantityInternal(Quantity q)
    {
        if (!_quantities.Contains(q))
            _quantities.Add(q);
    }
 
    public void AddQuantity(Quantity q)
    {
        if (q == null)
            throw new ArgumentException("Quantity cannot be null.");

        if (_quantities.Contains(q))
            throw new InvalidOperationException("Quantity already added to this MenuItem.");

        if (q.Item != this)
            throw new InvalidOperationException("Quantity belongs to a different MenuItem.");

        _quantities.Add(q);

        // only add one-way to prevent recursion and duplication
        if (!q.Order.Quantities.Contains(q))
            q.Order.AddQuantityInternal(q);
    }

 
    // ============================
    // MENU ASSOCIATION (1..*)
    // ============================
 
    private List<Menu> _menus = new();
    public IReadOnlyList<Menu> Menus => _menus.AsReadOnly();
 
    internal void AddMenuInternal(Menu menu)
    {
        if (!_menus.Contains(menu))
            _menus.Add(menu);
    }
 
    internal void RemoveMenuInternal(Menu menu)
    {
        if (_menus.Contains(menu))
            _menus.Remove(menu);
    }
 
    public void AddMenu(Menu menu)
    {
        if (menu == null)
            throw new ArgumentException("Menu cannot be null.");
 
        if (_menus.Contains(menu))
            throw new InvalidOperationException("Menu already linked to this MenuItem.");
 
        _menus.Add(menu);
 
        if (!menu.Items.Contains(this))
            menu.AddMenuItem(this);
    }
 
    // ============================
    // INGREDIENT AGGREGATION  (1..* —— 1..*)
    // ============================
 
    private List<Ingredient> _ingredients = new();
    public IReadOnlyList<Ingredient> Ingredients => _ingredients.AsReadOnly();
 
    internal void AddIngredientInternal(Ingredient ing)
    {
        if (!_ingredients.Contains(ing))
            _ingredients.Add(ing);
    }
 
    public void AddIngredient(Ingredient ing)
    {
        if (ing == null)
            throw new ArgumentException("Ingredient cannot be null.");
 
        if (_ingredients.Contains(ing))
            throw new InvalidOperationException("Ingredient already added to this MenuItem.");
 
        _ingredients.Add(ing);
 
        if (!ing.MenuItems.Contains(this))
            ing.AddMenuItemInternal(this);
    }
 
    // ============================
    // ATTRIBUTES
    // ============================
 
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty");
            if (value.Length < 2 || value.Length > 50)
                throw new ArgumentException("Name length must be between 2 and 50 characters");
            _name = value.Trim();
        }
    }
 
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0m || value > 1000m)
                throw new ArgumentException("Price must be between 0 and 1000");
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
            if (value is not null)
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Description cannot be blank");
                if (value.Length > 200)
                    throw new ArgumentException("Description is too long");
 
                _description = value.Trim();
            }
            else
            {
                _description = null;
            }
        }
    }
 
    // ============================
    // CONSTRUCTOR
    // ============================
 
    protected MenuItems(string name, decimal price, bool isAvailable, string? description = null)
    {
        ItemId = _nextId++;
 
        Name = name;
        Price = price;
        IsAvailable = isAvailable;
        Description = description;
 
        AddToExtent(this);
    }
 
    private static void AddToExtent(MenuItems item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        _extent.Add(item);
    }
 
    public static void ClearExtentForTests()
    {
        _extent.Clear();
        _nextId = 1; 
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