using System.Text.Json;
using Main.Classes.Employees;

[Serializable]
public class Chef : Staff
{
    private static List<Chef> _chefExtent = new List<Chef>();

    private static void AddToExtent(Chef chef)
    {
        if (chef == null)
            throw new ArgumentException("Chef cannot be null.");
        _chefExtent.Add(chef);
    }

    public static IReadOnlyList<Chef> GetExtent()
    {
        return _chefExtent.AsReadOnly();
    }


    public Chef(int staffId,string firstName, string lastName, decimal salary, string department)
        : base(staffId,firstName, lastName, salary, department)
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

    public static void Save(string path = "chefs.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_chefExtent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save chefs.", ex);
        }
    }

    public static bool Load(string path = "chefs.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _chefExtent.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            _chefExtent = JsonSerializer.Deserialize<List<Chef>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            _chefExtent.Clear();
            return false;
        }
    }
}