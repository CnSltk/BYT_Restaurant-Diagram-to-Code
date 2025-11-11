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

    public void ReceivePayment()
    {
        Console.WriteLine($"Cashier {LastName} is receiving payment.");
    }

    public void IssueRefund()
    {
        Console.WriteLine($"Cashier {LastName} is issuing a refund.");
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