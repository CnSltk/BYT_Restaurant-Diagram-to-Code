using Main.Classes.Orders;
using Menu;

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

        // Sadece tek yönlü çağrı
        order.AddQuantity(this);
    }
}