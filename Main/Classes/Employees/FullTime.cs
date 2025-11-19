using System.Collections.ObjectModel;
using System.Text.Json;

namespace Main.Classes.Employees;

public enum Shift
{
    Morning, Evening, Night
}

[Serializable]
public class FullTime : Staff
{
    private static List<FullTime> _fullTimeExtent = new List<FullTime>();
    
    private static void AddToExtent(FullTime fullTime)
    {
        if (fullTime == null)
            throw new ArgumentException("FullTime cannot be null.");
        _fullTimeExtent.Add(fullTime);
    }

    public static IReadOnlyList<FullTime> GetExtent()
    {
        return _fullTimeExtent.AsReadOnly();
    }
    public Shift Shift { get; private set; }

    public FullTime(string firstName, string lastName, decimal salary, string department, Shift shift)
        : base(firstName, lastName, salary, department)
    {
        Shift = shift;
    }

    public void ShiftChange(Shift newShift)
    {
        Shift = newShift;
    }

    public override void hireStaff()
    {
        throw new NotImplementedException();
    }

    public override void fireStaff()
    {
        throw new NotImplementedException();
    }
    
    public static void Save(string path = "fullTimes.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_fullTimeExtent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save FullTime.", ex);
        }
    }

    public static bool Load(string path = "fullTimes.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _fullTimeExtent.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            _fullTimeExtent = JsonSerializer.Deserialize<List<FullTime>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            _fullTimeExtent.Clear();
            return false;
        }
    }
}