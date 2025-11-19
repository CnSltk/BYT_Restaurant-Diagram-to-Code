using System.Text.Json;

namespace Main.Classes.Orders;

[Serializable]
public class Order
{
    
    private static List<Order> _extent = new List<Order>();

    public static IReadOnlyList<Order> Extent => _extent.AsReadOnly();

    public static void AddToExtent(Order order)
    {
        if (order == null)
            throw new ArgumentException("Order cannot be null.");

        if (!_extent.Contains(order))
            _extent.Add(order);
    }


    

    public static decimal MinPrice { get; } = 30m;

    private bool _isTakeAway;
    public bool IsTakeAway
    {
        get => _isTakeAway;
        set => _isTakeAway = value;
    }

    private OrderStatus _status;
    public OrderStatus Status
    {
        get => _status;
        set
        {
            if (!Enum.IsDefined(typeof(OrderStatus), value))
                throw new ArgumentException("Invalid order status.");

            _status = value;
        }
    }

    private DateTime _orderTime;
    public DateTime OrderTime
    {
        get => _orderTime;
        set
        {
            // Future orders should not exist
            if (value > DateTime.Now.AddMinutes(5))
                throw new ArgumentException("Order time cannot be in the far future.");

            _orderTime = value;
        }
    }

    // Read-only computed attribute
    public TimeSpan OrderPrepDuration => DateTime.Now - OrderTime;


   
    public Order(DateTime orderTime, bool isTakeAway, OrderStatus status)
    {
        OrderTime = orderTime;
        IsTakeAway = isTakeAway;
        Status = status;

        AddToExtent(this);
    }


   

    public void PlaceOrder(Customer customer)
    {
        if (customer == null)
            throw new ArgumentException("Customer cannot be null.");

        // Business logic would go here
        Console.WriteLine("Order placed.");
    }

    public void CalculateTotal()
    {
        Console.WriteLine("Total price calculated.");
    }

    public void TakeOrder()
    {
        Status = OrderStatus.Preparing;
        Console.WriteLine("Order has been taken.");
    }


    

    public static void SaveExtent(string path = "orders.json")
    {
        var json = JsonSerializer.Serialize(_extent, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public static void LoadExtent(string path = "orders.json")
    {
        if (!File.Exists(path))
        {
            _extent.Clear();
            return;
        }

        var json = File.ReadAllText(path);
        var loaded = JsonSerializer.Deserialize<List<Order>>(json);

        _extent = loaded ?? new List<Order>();
    }
}

public enum OrderStatus
{
    Preparing,
    Prepared,
    NotPrepared
}
