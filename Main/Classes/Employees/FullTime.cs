namespace Main.Classes.Employees;

public enum Shift
{
    Morning, Evening, Night
}

[Serializable]
public class FullTime : Staff
{
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
}