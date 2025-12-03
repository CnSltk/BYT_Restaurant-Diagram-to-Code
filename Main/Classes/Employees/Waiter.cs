using System.Text.Json;
using Main.Classes.Employees;
using Main.Classes.Restaurant;


[Serializable]
public class Waiter : Staff
{
    private static List<Waiter> _waiterExtent = new List<Waiter>();

    
    
    /*
     * Make this table an object for the table class. This is for the skeleton
     */
    private int _tables;
    public int Tables
    {
        get { return _tables; }
        set
        {
            if(value < 0)
                throw new ArgumentException("Table cannot be negative");
            _tables = value;
        }
    }

    private static void AddToExtent(Waiter waiter)
    {
        if (waiter == null)
            throw new ArgumentException("Waiter cannot be null.");
        _waiterExtent.Add(waiter);
    }

    public static IReadOnlyList<Waiter> GetExtent()
    {
        return _waiterExtent.AsReadOnly();
    }


    public Waiter(int staffId,string firstName, string lastName, decimal salary, string department, int tables)
        : base(staffId,firstName, lastName, salary, department)
    {
        Tables = tables;
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

    public void SeatCustomer(Table table)
    {
        if (!table.IsOccupied)
        {
            Console.WriteLine("Customer seated");
        }
        else
        {
            Console.WriteLine("Table is ocupied");
        }
    }

    public static void Save(string path = "waiters.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_waiterExtent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save waiters.", ex);
        }
    }

    public static bool Load(string path = "waiters.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _waiterExtent.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            _waiterExtent = JsonSerializer.Deserialize<List<Waiter>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            _waiterExtent.Clear();
            return false;
        }
    }
}