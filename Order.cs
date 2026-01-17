namespace prg_asg;

public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDateTime { get; set; }
    public double OrderTotal { get; set; }
    public string OrderStatus { get; set; }
    public DateTime DeliveryDateTime { get; set; }
    public string DeliveryAddress { get; set; }
    public string OrderPaymentMethod { get; set; }
    public bool OrderPaid { get; set; }
    public List<OrderedFoodItem> OrderedFoodItems { get; set; }

    public Order()
    {
        OrderId = new Random().Next();
        OrderDateTime = DateTime.Now;
        OrderTotal = CalculateOrderTotal();
        OrderStatus = "Pending";
        OrderPaid = false;
    }

    public double CalculateOrderTotal()
    {
        double total = 0;
        foreach (OrderedFoodItem orderedFoodItem in OrderedFoodItems)
        {
            total += orderedFoodItem.CalculateSubtotal();
        }
        return total;
    }
    public void AddOrderedFoodItem(OrderedFoodItem orderedFoodItem)
    {
        OrderedFoodItems.Add(orderedFoodItem);
    }
    public bool RemoveOrderedFoodItem(OrderedFoodItem orderedFoodItem)
    {
        bool removed = OrderedFoodItems.Remove(orderedFoodItem);
        return removed;
    }
    public void DisplayOrderedFoodItems()
    {
        foreach (OrderedFoodItem orderedFoodItem in OrderedFoodItems)
        {
            Console.WriteLine(orderedFoodItem.ToString());
        }
    }
    public override string ToString()
    {
        return
          "OrderId: " + OrderId +
          " OrderDateTime: " + OrderDateTime +
          " OrderTotal: " + OrderTotal +
          " OrderStatus: " + OrderStatus +
          " DeliveryDateTime: " + DeliveryDateTime +
          " DeliveryAddress: " + DeliveryAddress +
          " OrderPaymentMethod: " + OrderPaymentMethod +
          " OrderPaid: " + OrderPaid;
    }


}
