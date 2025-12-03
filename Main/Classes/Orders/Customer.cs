using System.Text.Json;

namespace Main.Classes.Orders;

[Serializable]
public class Customer
{
    private static List<Customer> _extent = new List<Customer>();
    public static IReadOnlyList<Customer> Extent => _extent.AsReadOnly();

    public static void AddToExtent(Customer c)
    {
        if (c == null)
            throw new ArgumentException("Customer cannot be null.");

        if (!_extent.Contains(c))
            _extent.Add(c);
    }

    public static void SaveExtent(string path = "customer.json")
    {
        var json = JsonSerializer.Serialize(_extent);
        File.WriteAllText(path, json);
    }

    public static void LoadExtent(string path = "customer.json")
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Extent file not found.");

        var json = File.ReadAllText(path);
        var loaded = JsonSerializer.Deserialize<List<Customer>>(json);

        if (loaded != null)
            _extent = loaded;
    }

    // ============================
    // ATTRIBUTES
    // ============================

    private int _customerId;
    public int CustomerId
    {
        get => _customerId;
        set
        {
            if (value < 1)
                throw new ArgumentException("CustomerId must be greater than 0.");
            _customerId = value;
        }
    }

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty.");

            if (value.Length < 2)
                throw new ArgumentException("Name must have at least 2 characters.");

            _name = value.Trim();
        }
    }

    private string _surname;
    public string Surname
    {
        get => _surname;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Surname cannot be empty.");

            if (value.Length < 2)
                throw new ArgumentException("Surname must have at least 2 characters.");

            _surname = value.Trim();
        }
    }

    private string? _phoneNumber;
    public string? PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            if (value != null)
            {
                if (value.Length < 6)
                    throw new ArgumentException("Phone number must have at least 6 digits.");
                if (!value.All(char.IsDigit))
                    throw new ArgumentException("Phone number must contain only digits.");
            }

            _phoneNumber = value;
        }
    }

    private string? _email;
    public string? Email
    {
        get => _email;
        set
        {
            if (value != null)
            {
                if (!value.Contains("@") || !value.Contains("."))
                    throw new ArgumentException("Invalid email format.");
            }

            _email = value;
        }
    }

    // ============================
    // ASSOCIATION: 1 Customer -- * Orders
    // ============================

    private List<Order> _orders = new List<Order>();
    public IReadOnlyList<Order> Orders => _orders.AsReadOnly();

    public void AddOrder(Order order)
    {
        if (order == null)
            throw new ArgumentException("Order cannot be null.");

        if (!_orders.Contains(order))
        {
            _orders.Add(order);

            if (order.Customer != this)
                order.Customer = this;
        }
    }

    // ============================
    // CONSTRUCTOR
    // ============================

    public Customer(int customerId, string name, string surname, string? phoneNumber, string? email)
    {
        CustomerId = customerId;
        Name = name;
        Surname = surname;
        PhoneNumber = phoneNumber;
        Email = email;

        AddToExtent(this);
    }
}
