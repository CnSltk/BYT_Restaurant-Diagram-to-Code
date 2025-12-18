using Menu;

namespace Main.Classes.Orders;

[Serializable]
public class Order
{
    private static List<Order> _extent = new();
    public static IReadOnlyList<Order> Extent => _extent.AsReadOnly();

    public static void ClearExtentForTests()
    {
        _extent.Clear();
    }

    public Order(DateTime orderTime, bool isTakeAway, OrderStatus status, Customer customer)
    {
        OrderTime = orderTime;
        IsTakeAway = isTakeAway;
        Status = status;
        Customer = customer;

        _extent.Add(this);
    }

    public bool IsTakeAway { get; set; }

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
                throw new ArgumentException("Order time cannot be in future.");
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
                throw new ArgumentException("Customer cannot be null.");

            _customer = value;

            if (!value.Orders.Contains(this))
                value.AddOrder(this);
        }
    }

    // =========================
    // QUANTITY ASSOCIATION (OWNER SIDE)
    // =========================
    private List<Quantity> _quantities = new();
    public IReadOnlyList<Quantity> Quantities => _quantities.AsReadOnly();

    // ✅ MUST BE PUBLIC because Quantity calls it
    public void AddQuantity(Quantity q)
    {
        if (q == null)
            throw new ArgumentException("Quantity cannot be null.");

        if (_quantities.Contains(q))
            throw new InvalidOperationException("Quantity already exists in Order.");

        if (q.Order != this)
            throw new InvalidOperationException("Quantity belongs to a different Order.");

        _quantities.Add(q);

        // ✅ update passive side
        q.Item.AddQuantityInternal(q);
    }

    public void RemoveQuantity(Quantity q)
    {
        if (q == null)
            throw new ArgumentException("Quantity cannot be null.");

        if (!_quantities.Contains(q))
            throw new InvalidOperationException("Quantity not found.");

        if (_quantities.Count == 1)
            throw new InvalidOperationException("Order must contain at least one Quantity.");

        _quantities.Remove(q);

        // ✅ update passive side
        q.Item.RemoveQuantityInternal(q);
    }
}

public enum OrderStatus
{
    Preparing,
    Prepared,
    NotPrepared
}
