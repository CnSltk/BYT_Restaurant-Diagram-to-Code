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
}