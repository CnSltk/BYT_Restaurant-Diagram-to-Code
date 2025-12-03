using Main.Classes.Menu;
namespace Main.Classes.Orders;

public class Quantity
{
    public Order Order { get; private set; }
    public MenuItems Item { get; private set; }
    public int Amount { get; private set; }

    public Quantity(Order order, MenuItems item, int amount)
    {
        if (order == null)
            throw new ArgumentException("Order cannot be null.");
        if (item == null)
            throw new ArgumentException("MenuItem cannot be null.");
        if (amount <= 0)
            throw new ArgumentException("Quantity must be greater than 0.");

        Order = order;
        Item = item;
        Amount = amount;

        // Register with Order
        if (!order.Quantities.Contains(this))
            order.AddQuantity(this);

        // Register with MenuItem
        if (!item.Quantities.Contains(this))
            item.AddQuantity(this);
    }
}