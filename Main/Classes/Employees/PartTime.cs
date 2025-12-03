using System.Text.Json;

namespace Main.Classes.Employees;

[Serializable]
public class PartTime : Staff
{
    private int _hours;
    private decimal _hourlyRate;
    
    private static List<PartTime> _partTimeExtent = new List<PartTime>();
    
    private static void AddToExtent(PartTime partTime)
    {
        if (partTime == null)
            throw new ArgumentException("PartTime cannot be null.");
        _partTimeExtent.Add(partTime);
    }

    public static IReadOnlyList<PartTime> GetExtent()
    {
        return _partTimeExtent.AsReadOnly();
    }

    public int Hours
    {
        get => _hours;
        set
        {
            if (value <= 0) 
                throw new ArgumentException("Hours can't be zero or negative");
            _hours = value;
            Salary = WeeklySalary;
        }
    }
    public decimal HourlyRate
    {
        get => _hourlyRate;
        set
        {
            if (value <= 0) 
                throw new ArgumentException("HourlyRate can't be zero or negative");
            _hourlyRate = value;
            Salary = WeeklySalary;
        }
    }
    public decimal WeeklySalary => Hours * HourlyRate;

    public PartTime(int staffId,string firstName, string lastName, string department, int hours, decimal hourlyRate)
        : base(staffId,firstName, lastName,0m ,department)
    {
        Hours = hours;
        HourlyRate = hourlyRate;
        Salary = WeeklySalary;
    }
    public override void hireStaff()
    {
        throw new NotImplementedException();
    }

    public override void fireStaff()
    {
        throw new NotImplementedException();
    }
    
    public static void Save(string path = "partTimes.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_partTimeExtent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save PartTime.", ex);
        }
    }

    public static bool Load(string path = "partTimes.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _partTimeExtent.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            _partTimeExtent = JsonSerializer.Deserialize<List<PartTime>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            _partTimeExtent.Clear();
            return false;
        }
    }
}