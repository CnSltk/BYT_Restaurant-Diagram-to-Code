using System.Text.Json;
using Main.Classes.Employees;

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
    private static List<Table> _extent = new List<Table>();
    public Table(int tableId, int number)
    {
        if(tableId <= 0)
            throw new ArgumentException("Table id can't be zero or negative");
        TableId = tableId;
        Number=number;
        AddToExtent(this);
    }

    private static void AddToExtent(Table table)
    {
        if(table==null)
            throw new ArgumentNullException(nameof(table));
        _extent.Add(table);
    }

    public static IReadOnlyList<Table> GetExtent() 
        => _extent.AsReadOnly();
    public static void ClearExtentForTests()
        => _extent.Clear();
    
    public static void Save(string path = "Table.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_extent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save Table.", ex);
        }
    }

    public static bool Load(string path = "Table.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                _extent.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            _extent = JsonSerializer.Deserialize<List<Table>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            _extent.Clear();
            return false;
        }
    }
}