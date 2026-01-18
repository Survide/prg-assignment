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
    public Restaurant FromRestaurant { get; set; }
    public Customer FromCustomer { get; set; }
    public SpecialOffer Offer { get; set; }

    public Order(
        int orderId,
        DateTime orderDateTime,
        DateTime deliveryDateTime,
        string address,
        double total,
        string status,
        Customer fromCustomer,
        Restaurant fromRestaurant,
        SpecialOffer offer,
        List<OrderedFoodItem> orderedFoodItems
    )
    {
        OrderId = orderId;
        OrderDateTime = orderDateTime;
        OrderTotal = total;
        OrderStatus = status;

        FromCustomer = fromCustomer;
        FromRestaurant = fromRestaurant;

        DeliveryDateTime = deliveryDateTime;
        DeliveryAddress = address;
        OrderPaymentMethod = "";
        OrderPaid = false;

        Offer = offer;
        OrderedFoodItems = orderedFoodItems;
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
