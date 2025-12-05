using System;
using System.Text.Json.Serialization;

namespace Main.Classes.Employees;

[Serializable]
public class ShiftAssociation
{
    public ShiftType CurrentShift { get; }
    public ShiftType ChangedShift { get; private set; }

    private readonly Staff _staff;
    public Staff Staff => _staff;
    
    private readonly Restaurant.Restaurant _restaurant;
    public Restaurant.Restaurant Restaurant => _restaurant;

    private ShiftAssociation(Staff staff, Restaurant.Restaurant restaurant, ShiftType shiftType)
    {
        if (staff == null || restaurant == null)
            throw new ArgumentNullException("Both staff and restaurant are required");
            
        _staff = staff;
        _restaurant = restaurant;
        CurrentShift = shiftType;
        ChangedShift = shiftType;
    }

    public static ShiftAssociation Create(Staff staff, Restaurant.Restaurant restaurant, ShiftType shiftType)
    {
        var association = new ShiftAssociation(staff, restaurant, shiftType);
        staff.AddShiftAssociation(association);
        restaurant.AddShiftAssociation(association);
        return association;
    }

    public void ChangeShift(ShiftType newShift) => ChangedShift = newShift;

    public void Remove()
    {
        _staff.RemoveShiftAssociation(this);
        _restaurant.RemoveShiftAssociation(this);
    }
}

public enum ShiftType { Morning, Afternoon, Evening, Night }