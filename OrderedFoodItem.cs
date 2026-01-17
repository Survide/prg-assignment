namespace prg_asg;

public class OrderedFoodItem : FoodItem
{
    public int QtyOrdered { get; set; }
    public double SubTotal { get; set; }

    public OrderedFoodItem(
        string itemName,
        string itemDesc,
        double itemPrice,
        int qty
    ) : base(itemName, itemDesc, itemPrice, "")
    {
        QtyOrdered = qty;
        SubTotal = CalculateSubtotal();
    }

    public double CalculateSubtotal()
    {
        return ItemPrice * QtyOrdered;
    }

}
