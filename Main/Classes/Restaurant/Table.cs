using System;
using System.Collections.Generic;
using System.Linq;

namespace Main.Classes.Restaurant;

[Serializable]
public class Table
{
    public int TableId { get; }
    
    private int _number;
    public int Number
    {
        get => _number;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Table number can't be zero or negative");
            _number = value;
        }
    }

    private bool _isOccupied = false;
    public bool IsOccupied
    {
        get => _isOccupied;
        set => _isOccupied = value;
    }

    // 1. DIRECT OBJECT REFERENCE (not ID)
    private Restaurant _restaurant;
    public Restaurant Restaurant
    {
        get => _restaurant;
        private set => _restaurant = value; // Private setter
    }

    // EXTENT: Using List with manual duplicate check
    private static readonly List<Table> _extent = new();

    // Constructor establishes composition
    public Table(int tableId, int number, Restaurant restaurant)
    {
        if(tableId <= 0)
            throw new ArgumentException("Table id can't be zero or negative");
        if(restaurant == null)
            throw new ArgumentNullException(nameof(restaurant));
            
        TableId = tableId;
        Number = number;
        
        // 2. ESTABLISH REVERSE CONNECTION
        SetRestaurant(restaurant);
        AddToExtent(this);
    }

    internal static void AddToExtent(Table table)
    {
        if(table == null)
            throw new ArgumentNullException(nameof(table));
        
        // MANUAL DUPLICATE CHECK (HashSet does this automatically)
        if (_extent.Any(t => t.TableId == table.TableId))
            throw new ArgumentException($"Table with ID {table.TableId} already exists");
            
        _extent.Add(table);
    }

    // Internal method for reverse connection management
    internal void SetRestaurant(Restaurant restaurant)
    {
        if (restaurant == null)
            throw new ArgumentNullException(nameof(restaurant));
        
        // Remove from old restaurant if reassigning
        if (_restaurant != null && _restaurant != restaurant)
        {
            _restaurant.RemoveTableFromCollection(this);
        }
        
        _restaurant = restaurant;
        restaurant.AddTableToCollection(this); // Reverse connection
    }

    // Return defensive copy
    public static IReadOnlyCollection<Table> GetExtent() => _extent.AsReadOnly();
    
    public static IReadOnlyCollection<Table> GetByRestaurant(Restaurant restaurant)
    {
        if (restaurant == null)
            throw new ArgumentNullException(nameof(restaurant));
        return _extent.Where(t => t._restaurant == restaurant).ToList().AsReadOnly();
    }

    public static void ClearExtentForTests() => _extent.Clear();
    
    // For cascade delete
    internal static void RemoveFromExtent(Table table) => _extent.Remove(table);
}