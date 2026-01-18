namespace prg_asg;

public class OrderedFoodItem : FoodItem
{
    public int QtyOrdered { get; set; }
    public double SubTotal { get; set; }

    public OrderedFoodItem(FoodItem item, int qty): base(item.ItemName, item.ItemDesc, item.ItemPrice, item.Customise) {
        QtyOrdered = qty;
    }

    public double CalculateSubtotal()
    {
        return ItemPrice * QtyOrdered;
    }

}
