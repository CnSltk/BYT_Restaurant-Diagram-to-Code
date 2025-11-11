namespace Menu;

[Serializable]
public class Menu
{
    private string _name;
    public Guid MenuId { get; }
    public string Version { get; set; }
    public bool IsActive { get; set; }

    private static List<Menu> _extent = new();

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Menu name cannot be empty");
            _name = value.Trim();
        }
    }

    public Menu(string name, string version, bool isActive)
    {
        MenuId = Guid.NewGuid();
        Name = name;
        Version = version;
        IsActive = isActive;
        AddToExtent(this);
    }

    private static void AddToExtent(Menu menu)
    {
        if (menu == null)
            throw new ArgumentNullException(nameof(menu));
        _extent.Add(menu);
    }

    public static IReadOnlyList<Menu> GetExtent() => _extent.AsReadOnly();

    public static void ClearExtentForTest() => _extent.Clear();
}