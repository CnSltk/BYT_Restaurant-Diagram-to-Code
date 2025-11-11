using System.Text.Json;
using Main.Classes.Employees;

[Serializable]
public class Manager : Staff
{
    private static List<Manager> _managerExtent = new List<Manager>();

    private static void AddToExtent(Manager manager)
    {
        if (manager == null)
            throw new ArgumentException("Manager cannot be null.");
        _managerExtent.Add(manager);
    }

    public static IReadOnlyList<Manager> GetExtent()
    {
        return _managerExtent.AsReadOnly();
    }
    
    public ManagerLevels Level { get; set; }

    public Manager(string firstName, string lastName, decimal salary, string department, ManagerLevels level)
        : base(firstName, lastName, salary, department)
    {
        Level = level;
        AddToExtent(this);
    }

    public override void hireStaff()
    {
        Console.WriteLine($"Manager {LastName} is hiring staff.");
    }

    public override void fireStaff()
    {
        Console.WriteLine($"Manager {LastName} is firing staff.");
    }
    

    public void ManageEmployee()
    {
        Console.WriteLine($"Manager {LastName} is managing employees.");
    }

    public void ChangeStaffShift()
    {
        Console.WriteLine($"Manager {LastName} is changing staff shift.");
    }

    public static void Save(string path = "managers.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_managerExtent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save managers.", ex);
        }
    }

    public static bool Load(string path = "managers.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _managerExtent.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            _managerExtent = JsonSerializer.Deserialize<List<Manager>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            _managerExtent.Clear();
            return false;
        }
    }
}