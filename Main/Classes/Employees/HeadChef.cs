using System.Text.Json;
namespace Main.Classes.Employees;

[Serializable]
public class HeadChef : Chef
{
    private static List<HeadChef> _headChefExtent = new List<HeadChef>();

    private static void AddToExtent(HeadChef headChef)
    {
        if (headChef == null)
            throw new ArgumentException("HeadChef cannot be null.");
        _headChefExtent.Add(headChef);
    }

    public static IReadOnlyList<HeadChef> GetExtent()
    {
        return _headChefExtent.AsReadOnly();
    }

    public SignatureDish Dish { get; set; }

    public HeadChef(int staffId,string firstName, string lastName, decimal salary, string department, SignatureDish dish)
        : base(staffId,firstName, lastName, salary, department)
    {
        Dish = dish;
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

    public void ManageInventory()
    {
        Console.WriteLine($"Head Chef {LastName} is managing inventory.");
    }

    public static new void Save(string path = "headchefs.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_headChefExtent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save head chefs.", ex);
        }
    }

    public static new bool Load(string path = "headchefs.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _headChefExtent.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            _headChefExtent = JsonSerializer.Deserialize<List<HeadChef>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            _headChefExtent.Clear();
            return false;
        }
    }
}