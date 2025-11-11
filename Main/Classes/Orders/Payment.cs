using System.Text.Json;

namespace Main.Classes.Orders;

[Serializable]
public class Payment
{
    
    private static List<Payment> Payment_lists = new List<Payment>();
    private static void AddToExtent(Payment payment)
    {
        if (payment == null)
            throw new ArgumentException("Payment cannot be null.");
        Payment_lists.Add(payment);
    }
    public static IReadOnlyList<Payment> GetExtent()
    {
        return Payment_lists.AsReadOnly();
    }
    
    public int PaymentID { get; set; }
    public PaymentMethod Method {get; set;}
    public PaymentStatus Status { get; set; }
    private Decimal _amount;

    public Decimal Amount
    {
        get => _amount;
        set
        {
            if (value < 0)
            {
                throw new Exception("Amount cannot be less than 0");
            }
            _amount = value;
        }
    }
    public DateTime? PaidAt { get; set; }
    
    private DateTime _paymenTime;
    public DateTime PaymentTime
    {
        get => _paymenTime;
        set
        {
            if (value > DateTime.Now)
            {
                throw new Exception("Payment time cannot be in the future");
            }
            _paymenTime = value;
        }
    }

    public Payment(decimal amount, DateTime paymenTime, PaymentMethod method, PaymentStatus status)
    {
        _amount = amount;
        _paymenTime = paymenTime;
        Method = method;
        Status = status;
    }

    public void Pay()
    {
        Console.WriteLine("Payment paid");
    }
    
    public static void Save(string path = "payments.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(Payment_lists, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save payments.", ex);
        }
    }
    
    public static bool Load(string path = "payments.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                Payment_lists.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            Payment_lists = JsonSerializer.Deserialize<List<Payment>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            Payment_lists.Clear();
            return false;
        }
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