using System.Text.Json;
using Menu;
 
namespace Main.Classes.Orders;
 
[Serializable]
public class Order
{
    private static List<Order> _extent = new();
    public static IReadOnlyList<Order> Extent => _extent.AsReadOnly();
 
    public static void AddToExtent(Order order)
    {
        if (order == null)
            throw new ArgumentException("Order cannot be null.");
 
        if (!_extent.Contains(order))
            _extent.Add(order);
    }
    public static void ClearExtentForTests()
    {
        _extent.Clear();
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
            if (value > DateTime.Now.AddMinutes(5))
                throw new ArgumentException("Order time cannot be in the far future.");
            _orderTime = value;
        }
    }
 
    public TimeSpan OrderPrepDuration => DateTime.Now - OrderTime;
 
    private Customer _customer;
    public Customer Customer
    {
        get => _customer;
        set
        {
            if (value == null)
                throw new ArgumentException("Order must have a Customer.");
 
            _customer = value;
 
            if (!value.Orders.Contains(this))
                value.AddOrder(this);
        }
    }
 
    private List<Payment> _payments = new();
    public IReadOnlyList<Payment> Payments => _payments.AsReadOnly();
 
    public void AddPayment(Payment payment)
    {
        if (payment == null)
            throw new ArgumentException("Payment cannot be null.");
 
        if (!_payments.Contains(payment))
        {
            _payments.Add(payment);
 
            if (payment.Order != this)
                payment.Order = this;
        }
    }
 
    private Delivery? _delivery;
    public Delivery? Delivery
    {
        get => _delivery;
        set
        {
            _delivery = value;
 
            if (value != null && value.Order != this)
                value.Order = this;
        }
    }
 
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
            throw new InvalidOperationException("Quantity already added to this Order.");

        if (q.Order != this)
            throw new InvalidOperationException("Quantity belongs to a different Order.");

        _quantities.Add(q);

        if (!q.Item.Quantities.Contains(q))
            q.Item.AddQuantityInternal(q);
    }

 
    public void RemoveQuantity(Quantity q)
    {
        if (!_quantities.Contains(q))
            throw new InvalidOperationException("Quantity not found in this Order.");
 
        if (_quantities.Count == 1)
            throw new InvalidOperationException("Order must contain at least one Quantity.");
 
        _quantities.Remove(q);
    }
 
    public Order(DateTime orderTime, bool isTakeAway, OrderStatus status, Customer customer)
    {
        OrderTime = orderTime;
        IsTakeAway = isTakeAway;
        Status = status;
        Customer = customer;
 
        AddToExtent(this);
    }
 
    public void PlaceOrder(Customer customer)
    {
        if (customer == null)
            throw new ArgumentException("Customer cannot be null.");
 
        Customer = customer;
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
        var json = JsonSerializer.Serialize(_extent,
            new JsonSerializerOptions { WriteIndented = true });
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