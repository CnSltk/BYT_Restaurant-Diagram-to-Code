using System.Text.Json;

namespace Menu;

[Serializable]
public class MenuItems
{
    // ============================
    // EXTENT
    // ============================
    private static List<MenuItems> _extent = new();
    public static IReadOnlyList<MenuItems> Extent => _extent.AsReadOnly();

    private static int _nextId = 1;

    public static void ClearExtentForTests()
    {
        _extent.Clear();
        _nextId = 1;
    }

    public static void Save(string path = "MenuItems.json")
    {
        var json = JsonSerializer.Serialize(_extent, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public static bool Load(string path = "MenuItems.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _extent.Clear();
                _nextId = 1;
                return false;
            }

            var json = File.ReadAllText(path);
            var loaded = JsonSerializer.Deserialize<List<MenuItems>>(json) ?? new List<MenuItems>();

            _extent = loaded;
            _nextId = (_extent.Count == 0) ? 1 : _extent.Max(x => x.ItemId) + 1;

            return true;
        }
        catch
        {
            _extent.Clear();
            _nextId = 1;
            return false;
        }
    }

    private static void AddToExtent(MenuItems item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        _extent.Add(item);
    }

    // ============================
    // ID + BASE ATTRIBUTES
    // ============================
    public int ItemId { get; private set; }

    private string _name = string.Empty;
    private decimal _price;
    private bool _isAvailable;
    private string? _description;

    public string Name
    {
        get => _name;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty");
            var trimmed = value.Trim();
            if (trimmed.Length < 2 || trimmed.Length > 50)
                throw new ArgumentException("Name length must be between 2 and 50 characters");
            _name = trimmed;
        }
    }

    public decimal Price
    {
        get => _price;
        private set
        {
            if (value < 0m || value > 1000m)
                throw new ArgumentException("Price must be between 0 and 1000");
            _price = Math.Round(value, 2);
        }
    }

    public bool IsAvailable
    {
        get => _isAvailable;
        private set => _isAvailable = value;
    }

    public string? Description
    {
        get => _description;
        private set
        {
            if (value is null)
            {
                _description = null;
                return;
            }

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Description cannot be blank");
            if (value.Length > 200)
                throw new ArgumentException("Description is too long");

            _description = value.Trim();
        }
    }

    public List<string> Allergens { get; set; } = new();

    // ============================
    // TYPE + CONDITIONAL ATTRIBUTES
    // ============================
    public MenuItemType ItemType { get; private set; }

    public bool? SpiceLevel { get; private set; }
    public bool? IsVegetarian { get; private set; }
    public int? Calories { get; private set; }
    public int? PrepTimeMin { get; private set; }
    public FoodCategory? FoodCategory { get; private set; }

    public int? VolumeMl { get; private set; }
    public BeverageCategory? BeverageCategory { get; private set; }

    // ============================
    // QUANTITY ASSOCIATION (PASSIVE SIDE)
    // ============================
    private List<Main.Classes.Orders.Quantity> _quantities = new();
    public IReadOnlyList<Main.Classes.Orders.Quantity> Quantities => _quantities.AsReadOnly();

    internal void AddQuantityInternal(Main.Classes.Orders.Quantity q)
    {
        if (q == null) throw new ArgumentException("Quantity cannot be null.");
        if (!_quantities.Contains(q))
            _quantities.Add(q);
    }

    internal void RemoveQuantityInternal(Main.Classes.Orders.Quantity q)
    {
        if (q == null) throw new ArgumentException("Quantity cannot be null.");
        _quantities.Remove(q);
    }

    // ============================
    // MENU ASSOCIATION
    // ============================
    private List<Menu> _menus = new();
    public IReadOnlyList<Menu> Menus => _menus.AsReadOnly();

    internal void AddMenuInternal(Menu menu)
    {
        if (!_menus.Contains(menu))
            _menus.Add(menu);
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

    internal void RemoveMenuInternal(Menu menu)
    {
        if (_menus.Contains(menu))
            _menus.Remove(menu);
    }

    public void RemoveMenu(Menu menu)
    {
        if (menu == null)
            throw new ArgumentException("Menu cannot be null.");

        if (!_menus.Contains(menu))
            throw new InvalidOperationException("Menu not found in this MenuItem.");

        _menus.Remove(menu);

        if (menu.Items.Contains(this))
            menu.RemoveMenuItem(this);
    }

    // ============================
    // INGREDIENT AGGREGATION
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

    internal void RemoveIngredientInternal(Ingredient ing)
    {
        if (_ingredients.Contains(ing))
            _ingredients.Remove(ing);
    }

    // ============================
    // REFLEX ASSOCIATION
    // ============================
    private List<MenuItems> _relatedItems = new();
    public IReadOnlyList<MenuItems> RelatedItems => _relatedItems.AsReadOnly();

    internal void AddRelatedItemInternal(MenuItems item)
    {
        if (!_relatedItems.Contains(item))
            _relatedItems.Add(item);
    }

    public void AddRelatedItem(MenuItems item)
    {
        if (item == null)
            throw new ArgumentException("Related menu item cannot be null.");

        if (item == this)
            throw new InvalidOperationException("Cannot relate a menu item to itself.");

        if (_relatedItems.Contains(item))
            throw new InvalidOperationException("Menu item already related.");

        _relatedItems.Add(item);

        if (!item._relatedItems.Contains(this))
            item.AddRelatedItemInternal(this);
    }

    internal void RemoveRelatedItemInternal(MenuItems item)
    {
        if (_relatedItems.Contains(item))
            _relatedItems.Remove(item);
    }

    public void RemoveRelatedItem(MenuItems item)
    {
        if (item == null)
            throw new ArgumentException("Related menu item cannot be null.");

        if (!_relatedItems.Contains(item))
            throw new InvalidOperationException("Related item not found.");

        _relatedItems.Remove(item);

        if (item._relatedItems.Contains(this))
            item.RemoveRelatedItemInternal(this);
    }

    // ============================
    // CONSTRUCTORS + FACTORIES
    // ============================
    private MenuItems() { }

    private MenuItems(string name, decimal price, bool isAvailable, string? description)
    {
        ItemId = _nextId++;
        Name = name;
        Price = price;
        IsAvailable = isAvailable;
        Description = description;

        AddToExtent(this);
    }

    public static MenuItems CreateFood(
        string name,
        decimal price,
        bool isAvailable,
        bool spiceLevel,
        bool isVegetarian,
        int calories,
        int prepTimeMin,
        FoodCategory category,
        string? description = null)
    {
        var item = new MenuItems(name, price, isAvailable, description)
        {
            ItemType = MenuItemType.Food,
            SpiceLevel = spiceLevel,
            IsVegetarian = isVegetarian,
            Calories = calories,
            PrepTimeMin = prepTimeMin,
            FoodCategory = category
        };

        item.ValidateConditionalAttributes();
        item.ValidateFoodValues();
        return item;
    }

    public static MenuItems CreateBeverage(
        string name,
        decimal price,
        bool isAvailable,
        int volumeMl,
        BeverageCategory beverageCategory,
        string? description = null)
    {
        var item = new MenuItems(name, price, isAvailable, description)
        {
            ItemType = MenuItemType.Beverage,
            VolumeMl = volumeMl,
            BeverageCategory = beverageCategory
        };

        item.ValidateConditionalAttributes();
        item.ValidateBeverageValues();
        return item;
    }

    private void ValidateConditionalAttributes()
    {
        if (ItemType == MenuItemType.Food)
        {
            if (VolumeMl != null || BeverageCategory != null)
                throw new InvalidOperationException("Beverage attributes must be null when ItemType = Food.");
        }
        else if (ItemType == MenuItemType.Beverage)
        {
            if (SpiceLevel != null || IsVegetarian != null || Calories != null || PrepTimeMin != null || FoodCategory != null)
                throw new InvalidOperationException("Food attributes must be null when ItemType = Beverage.");
        }
        else
        {
            throw new InvalidOperationException("Unknown MenuItemType.");
        }
    }

    private void ValidateFoodValues()
    {
        if (Calories is null || Calories <= 0 || Calories > 5000)
            throw new ArgumentException("Calories must be 1..5000 for Food items.");
        if (PrepTimeMin is null || PrepTimeMin < 0 || PrepTimeMin > 240)
            throw new ArgumentException("PrepTimeMin must be 0..240 for Food items.");
        if (FoodCategory is null)
            throw new ArgumentException("FoodCategory is required for Food items.");
        if (SpiceLevel is null || IsVegetarian is null)
            throw new ArgumentException("SpiceLevel and IsVegetarian are required for Food items.");
    }

    private void ValidateBeverageValues()
    {
        if (VolumeMl is null || (VolumeMl != 500 && VolumeMl != 1000))
            throw new ArgumentException("VolumeMl must be 500 or 1000 for Beverage items.");
        if (BeverageCategory is null)
            throw new ArgumentException("BeverageCategory is required for Beverage items.");
    }

    // Update methods (same as yours)
    public void UpdateBase(string? name = null, decimal? price = null, bool? isAvailable = null, string? description = null)
    {
        if (name is not null) Name = name;
        if (price is not null) Price = price.Value;
        if (isAvailable is not null) IsAvailable = isAvailable.Value;
        if (description is not null) Description = description;
    }

    public void UpdateFoodDetails(bool? spiceLevel = null, bool? isVegetarian = null, int? calories = null, int? prepTimeMin = null, FoodCategory? category = null)
    {
        if (ItemType != MenuItemType.Food)
            throw new InvalidOperationException("Cannot update food details for a Beverage item.");

        if (spiceLevel is not null) SpiceLevel = spiceLevel;
        if (isVegetarian is not null) IsVegetarian = isVegetarian;
        if (calories is not null) Calories = calories;
        if (prepTimeMin is not null) PrepTimeMin = prepTimeMin;
        if (category is not null) FoodCategory = category;

        ValidateConditionalAttributes();
        ValidateFoodValues();
    }

    public void UpdateBeverageDetails(int? volumeMl = null, BeverageCategory? beverageCategory = null)
    {
        if (ItemType != MenuItemType.Beverage)
            throw new InvalidOperationException("Cannot update beverage details for a Food item.");

        if (volumeMl is not null) VolumeMl = volumeMl;
        if (beverageCategory is not null) BeverageCategory = beverageCategory;

        ValidateConditionalAttributes();
        ValidateBeverageValues();
    }
}
