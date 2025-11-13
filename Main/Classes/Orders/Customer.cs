using System.Text.Json;

namespace Main.Classes.Orders;

[Serializable]
public class Customer
{
    private static List<Customer> Customers_list = new List<Customer>();
    private static void AddToExtent(Customer customer)
    {
        if (customer == null)
            throw new ArgumentException("Customer cannot be null.");
        Customers_list.Add(customer);
    }
    public static IReadOnlyList<Customer> GetExtent()
    {
        return Customers_list.AsReadOnly();
    }
    
    
    public int CustomerId { get; set; }

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new Exception("Name cannot be blank");
            }
            _name = value;
        }
    }
    
    private string _surname;
    public string Surname
    {
        get => _surname;
        set
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Surname cannot be empty");
            }
            _surname = value;
        }
    }
    
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public Customer(int customerId, string name, string surname,  string? phoneNumber, string? email)
    {
        if (customerId < 1)
            throw new ArgumentException("CustomerId cannot be less than 1.");
        CustomerId = customerId;
        Name = name;
        Surname = surname;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public static void Save(string path = "customers.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(Customers_list, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save customers.", ex);
        }
    }
    
    public static bool Load(string path = "customers.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                Customers_list.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            Customers_list = JsonSerializer.Deserialize<List<Customer>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            Customers_list.Clear();
            return false;
        }
    }
    
    
}