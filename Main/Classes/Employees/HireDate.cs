using System;
using System.Text.Json.Serialization;

namespace Main.Classes.Employees;

[Serializable]
public class HireDate
{
    public DateTime StartDate { get; }
    public DateTime? EndDate { get; private set; }
    
    public TimeSpan YearsOfService
    {
        get
        {
            DateTime endDate = EndDate ?? DateTime.Now;
            return endDate - StartDate;
        }
    }

    private readonly Staff _staff;
    public Staff Staff => _staff;
    
    private readonly Restaurant.Restaurant _restaurant;
    public Restaurant.Restaurant Restaurant => _restaurant;

    private HireDate(Staff staff, Restaurant.Restaurant restaurant, DateTime startDate)
    {
        if (staff == null || restaurant == null)
            throw new ArgumentNullException("Both staff and restaurant are required");
            
        _staff = staff;
        _restaurant = restaurant;
        StartDate = startDate;
        EndDate = null;
    }

    public static HireDate Create(Staff staff, Restaurant.Restaurant restaurant, DateTime startDate)
    {
        var hireDate = new HireDate(staff, restaurant, startDate);
        staff.AddHireDate(hireDate);
        restaurant.AddHireDate(hireDate);
        return hireDate;
    }

    public void Terminate() => EndDate = DateTime.Now;
    public bool IsCurrentlyEmployed => EndDate == null;

    internal void Remove()
    {
        _staff.RemoveHireDate(this);
        _restaurant.RemoveHireDate(this);
    }
}