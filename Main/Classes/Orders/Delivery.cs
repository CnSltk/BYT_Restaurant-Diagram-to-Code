using System.Text.Json;

namespace Main.Classes.Orders;

[Serializable]
public class Delivery
{
    private static List<Delivery> Delivery_lists = new List<Delivery>();
    private static void AddToExtent(Delivery delivery)
    {
        if (delivery == null)
            throw new ArgumentException("Delivery cannot be null.");
        Delivery_lists.Add(delivery);
    }
    public static IReadOnlyList<Delivery> GetExtent()
    {
        return Delivery_lists.AsReadOnly();
    }
    public int DeliveryID { get; set; }
    public DeliveryMethod Method { get; set; }
    public Adress Adress { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public Decimal? Fee { get; set; }
    public DeliveryStatus Status { get; set; }

    public Delivery(int deliveryId, DeliveryMethod method, Adress adress, DateTime? scheduledAt, DateTime? deliveredAt, decimal? fee, DeliveryStatus status)
    {
        DeliveryID = deliveryId;
        Method = method;
        Adress = adress;
        ScheduledAt = scheduledAt;
        DeliveredAt = deliveredAt;
        Fee = fee;
        Status = status;
    }

    public static void Save(string path = "delivery.json")
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(Delivery_lists, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to save deliveries.", ex);
        }
    }
    
    public static bool Load(string path = "delivery.json")
    {
        try
        {
            if (!File.Exists(path))
            {
                Delivery_lists.Clear();
                return false;
            }
            string jsonString = File.ReadAllText(path);
            Delivery_lists = JsonSerializer.Deserialize<List<Delivery>>(jsonString);
            return true;
        }
        catch (Exception)
        {
            Delivery_lists.Clear();
            return false;
        }
    }
}

public enum DeliveryMethod
{
    Courier,
    InRestaurant,
    Pickup
}

public class Adress
{
    public string StreetName { get; set; }
    public string City { get; set; }    
    public string ZipCode { get; set; }
}

public enum DeliveryStatus
{
    Scheduled,
    OnRoute,
    Delivered,
    Cancelled
}