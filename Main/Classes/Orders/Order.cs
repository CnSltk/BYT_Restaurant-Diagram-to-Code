using System.Text.Json;

namespace Main.Classes.Orders;

[Serializable]
public class Order
{
    private static List<Order> Orders_list = new List<Order>();
    private static void AddToExtent(Order order)
    {
        if (order == null)
            throw new ArgumentException("Manager cannot be null.");
        Orders_list.Add(order);
    }
    public static IReadOnlyList<Order> GetExtent()
    {
        return Orders_list.AsReadOnly();
    }
    
    public bool IsTakeAway{get;set;}
    public static Decimal MinPrice { get; } = 30;
    public OrderStatus Status {get;set;}

    private DateTime _orderTime;

    public DateTime OrderTime
    {
        get => _orderTime;
        set
        {
            _orderTime = value;
        }
    }

    public TimeSpan OrderPrepDuration
    {
        get => DateTime.Now - OrderTime;

    }

    public Order(DateTime orderTime, bool ısTakeAway, OrderStatus status)
    {
        _orderTime = orderTime;
        IsTakeAway = ısTakeAway;
        Status = status;
    }

    private static void AddOrder(Order order)
    {
        if (order == null)
        {
            throw new ArgumentException("Order cannot be null");
        }
        Orders_list.Add(order);
    }

    public void PlaceOrder(Customer customer, Order order)
    {
           Console.WriteLine("Order placed");
    }

    public void CalculateTotal()
    {
        Console.WriteLine("Total price calculated");
    }

    public void TakeOrder()
    {
        Console.WriteLine("Order has been taken");
    }
    
    public static void Save(string path = "orders.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(Orders_list, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save orders.", ex);
        }
    }
    
    public static bool Load(string path = "orders.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                Orders_list.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            Orders_list = JsonSerializer.Deserialize<List<Order>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            Orders_list.Clear();
            return false;
        }
    }
    
}

public enum OrderStatus
{
    Preparing,
    Prepared,
    NotPrepared
}