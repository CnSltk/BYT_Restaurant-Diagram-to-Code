using System.Text.Json;
using Main.Classes.Employees;

[Serializable]
public class Cashier : Staff
{
    private static List<Cashier> _cashierExtent = new List<Cashier>();

    private static void AddToExtent(Cashier cashier)
    {
        if (cashier == null)
            throw new ArgumentException("Cashier cannot be null.");
        _cashierExtent.Add(cashier);
    }

    public static IReadOnlyList<Cashier> GetExtent()
    {
        return _cashierExtent.AsReadOnly();
    }

    public Cashier(string firstName, string lastName, decimal salary, string department)
        : base(firstName, lastName, salary, department)
    {
        AddToExtent(this);
    }

    public override void hireStaff()
    {
        throw new NotImplementedException();
    }

    public override void fireStaff()
    {
        throw new NotImplementedException();
    }

    public void ReceivePayment(decimal amount, string paymentMethod, string orderId)
{
    if (amount <= 0)
        throw new ArgumentException("Payment amount must be positive.", nameof(amount));
    if (string.IsNullOrWhiteSpace(paymentMethod))
        throw new ArgumentException("Payment method cannot be empty.", nameof(paymentMethod));
    if (string.IsNullOrWhiteSpace(orderId))
        throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
    
    string transactionId = $"TXN-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
    Console.WriteLine($"[PAYMENT RECEIVED] Order: {orderId}, Amount: {amount:C}, Method: {paymentMethod}");
    Console.WriteLine($"[CONFIRMATION] Transaction ID: {transactionId}, Processed by Cashier: {LastName}");
}

public void IssueRefund(decimal amount, string orderId, string reason, string authorizationCode)
{
    if (amount <= 0)
        throw new ArgumentException("Refund amount must be positive.", nameof(amount));
    if (string.IsNullOrWhiteSpace(orderId))
        throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
    if (string.IsNullOrWhiteSpace(reason))
        throw new ArgumentException("Refund reason cannot be empty.", nameof(reason));
    if (string.IsNullOrWhiteSpace(authorizationCode))
        throw new ArgumentException("Manager authorization required.", nameof(authorizationCode));
    
    
    Console.WriteLine($"[REFUND ISSUED] Order: {orderId}, Amount: {amount:C}, Reason: {reason}");
    Console.WriteLine($"[AUTHORIZATION] Code: {authorizationCode}, Processed by Cashier: {LastName}");
}

    public static void Save(string path = "cashiers.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_cashierExtent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save cashiers.", ex);
        }
    }

    public static bool Load(string path = "cashiers.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _cashierExtent.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            _cashierExtent = JsonSerializer.Deserialize<List<Cashier>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            _cashierExtent.Clear();
            return false;
        }
    }
}