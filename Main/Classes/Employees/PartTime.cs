namespace Main.Classes.Employees;

[Serializable]
public class PartTime : Staff
{
    private int _hours;
    private decimal _hourlyRate;

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

    public PartTime(string firstName, string lastName, string department, int hours, decimal hourlyRate)
        : base(firstName, lastName,0m ,department)
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
}