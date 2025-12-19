    using System.Text.Json;

    namespace Main.Classes.Orders;

    [Serializable]
    public class Delivery
    {
        private static List<Delivery> _extent = new List<Delivery>();
        public static IReadOnlyList<Delivery> Extent => _extent.AsReadOnly();

        public static void AddToExtent(Delivery d)
        {
            if (d == null)
                throw new ArgumentException("Delivery cannot be null.");

            if (!_extent.Contains(d))
                _extent.Add(d);
        }
        
        public static void ClearExtentForTests()
        {
            _extent.Clear();
        }

        public static void SaveExtent(string path = "delivery.json")
        {
            var json = JsonSerializer.Serialize(_extent, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        public static void LoadExtent(string path = "delivery.json")
        {
            if (!File.Exists(path))
            {
                _extent.Clear();
                return;
            }

            var json = File.ReadAllText(path);
            var loaded = JsonSerializer.Deserialize<List<Delivery>>(json);

            _extent = loaded ?? new List<Delivery>();
        }

        private int _deliveryId;
        public int DeliveryID
        {
            get => _deliveryId;
            set
            {
                if (value < 1)
                    throw new ArgumentException("DeliveryID must be greater than 0.");
                _deliveryId = value;
            }
        }

        private DeliveryMethod _method;
        public DeliveryMethod Method
        {
            get => _method;
            set
            {
                if (!Enum.IsDefined(typeof(DeliveryMethod), value))
                    throw new ArgumentException("Invalid delivery method.");
                _method = value;
            }
        }

        private Adress _adress;
        public Adress Adress
        {
            get => _adress;
            set
            {
                if (value == null)
                    throw new ArgumentException("Adress cannot be null.");

                if (string.IsNullOrWhiteSpace(value.City))
                    throw new ArgumentException("City cannot be empty.");

                if (string.IsNullOrWhiteSpace(value.StreetName))
                    throw new ArgumentException("Street name cannot be empty.");

                if (string.IsNullOrWhiteSpace(value.ZipCode) || value.ZipCode.Length < 4)
                    throw new ArgumentException("ZipCode is invalid.");

                _adress = value;
            }
        }

        public DateTime? ScheduledAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        private decimal? _fee;
        public decimal? Fee
        {
            get => _fee;
            set
            {
                if (value != null && value < 0)
                    throw new ArgumentException("Fee cannot be negative.");
                _fee = value;
            }
        }

        public DeliveryStatus Status { get; set; }

        // ============================
        // ASSOCIATION: Delivery → Order (1)
        // ============================

        private Order _order;
        public Order Order
        {
            get => _order;
            set
            {
                if (value == null)
                    throw new ArgumentException("Delivery must belong to an Order.");

                _order = value;

                if (value.Delivery != this)
                    value.Delivery = this;
            }
        }

        // ============================
        // CONSTRUCTOR
        // ============================

        public Delivery(
            int deliveryId,
            DeliveryMethod method,
            Adress adress,
            DateTime? scheduledAt,
            DateTime? deliveredAt,
            decimal? fee,
            DeliveryStatus status,
            Order order)
        {
            DeliveryID = deliveryId;
            Method = method;
            Adress = adress;
            ScheduledAt = scheduledAt;
            DeliveredAt = deliveredAt;
            Fee = fee;
            Status = status;

            Order = order; // mandatory association

            AddToExtent(this);
        }
    }

    public class Adress
    {
        public string StreetName { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

        public Adress(string streetName, string city, string zipCode)
        {
            if (string.IsNullOrWhiteSpace(streetName))
                throw new ArgumentException("Street name cannot be empty.");

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty.");

            if (string.IsNullOrWhiteSpace(zipCode) || zipCode.Length < 4)
                throw new ArgumentException("ZipCode is invalid.");

            StreetName = streetName;
            City = city;
            ZipCode = zipCode;
        }

        public Adress() {}
    }


    public enum DeliveryMethod
    {
        Courier,
        InRestaurant,
        Pickup
    }

    public enum DeliveryStatus
    {
        Scheduled,
        OnRoute,
        Delivered,
        Cancelled
    }
