using System.Text.Json;

namespace Main.Classes.Orders;

[Serializable]
public class Payment
{
    private static List<Payment> _extent = new List<Payment>();
    public static IReadOnlyList<Payment> Extent => _extent.AsReadOnly();

    public static void AddToExtent(Payment payment)
    {
        if (payment == null)
            throw new ArgumentException("Payment cannot be null.");

        if (!_extent.Contains(payment))
            _extent.Add(payment);
    }
    public static void ClearExtentForTests()
    {
        _extent.Clear();
    }

    public static void SaveExtent(string path = "payments.json")
    {
        var json = JsonSerializer.Serialize(_extent, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public static void LoadExtent(string path = "payments.json")
    {
        if (!File.Exists(path))
        {
            _extent.Clear();
            return;
        }

        var json = File.ReadAllText(path);
        var loaded = JsonSerializer.Deserialize<List<Payment>>(json);

        _extent = loaded ?? new List<Payment>();
    }

    private int _paymentId;
    public int PaymentID
    {
        get => _paymentId;
        set
        {
            if (value < 1)
                throw new ArgumentException("PaymentID must be greater than 0.");
            _paymentId = value;
        }
    }

    private PaymentMethod _method;
    public PaymentMethod Method
    {
        get => _method;
        set
        {
            if (!Enum.IsDefined(typeof(PaymentMethod), value))
                throw new ArgumentException("Invalid payment method.");
            _method = value;
        }
    }

    private PaymentStatus _status;
    public PaymentStatus Status
    {
        get => _status;
        set
        {
            if (!Enum.IsDefined(typeof(PaymentStatus), value))
                throw new ArgumentException("Invalid payment status.");
            _status = value;
        }
    }

    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set
        {
            if (value < 0)
                throw new ArgumentException("Amount cannot be negative.");
            _amount = value;
        }
    }

    private DateTime _paymentTime;
    public DateTime PaymentTime
    {
        get => _paymentTime;
        set
        {
            if (value > DateTime.Now.AddMinutes(5))
                throw new ArgumentException("Payment time cannot be in the future.");
            _paymentTime = value;
        }
    }

    private DateTime? _paidAt;
    public DateTime? PaidAt
    {
        get => _paidAt;
        set
        {
            if (value != null && value < PaymentTime)
                throw new ArgumentException("PaidAt cannot be earlier than PaymentTime.");
            _paidAt = value;
        }
    }

    // ============================
    // ASSOCIATION: Payment → Order (1)
    // ============================

    private Order _order;
    public Order Order
    {
        get => _order;
        set
        {
            if (value == null)
                throw new ArgumentException("Payment must have an Order.");

            _order = value;

            if (!value.Payments.Contains(this))
                value.AddPayment(this);
        }
    }

    // ============================
    // CONSTRUCTOR
    // ============================

    public Payment(
        decimal amount,
        DateTime paymentTime,
        int paymentId,
        PaymentMethod method,
        PaymentStatus status,
        DateTime? paidAt,
        Order order)
    {
        Amount = amount;
        PaymentTime = paymentTime;
        PaymentID = paymentId;
        Method = method;
        Status = status;
        PaidAt = paidAt;

        Order = order;

        AddToExtent(this);
    }

    public void Pay()
    {
        Status = PaymentStatus.Completed;
        PaidAt = DateTime.Now;
        Console.WriteLine("Payment completed.");
    }
}

public enum PaymentMethod
{
    Cash,
    Card,
    Online
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}
