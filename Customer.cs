namespace prg_asg;

public class Customer
{
    public string EmailAddress { get; set; }
    public string CustomerName { get; set; }
    public List<Order> Orders { get; set; } = [];

    public Customer(string emailAddress, string customerName) 
    {
        EmailAddress = emailAddress;
        CustomerName = customerName;
    }

    public void AddOrder(Order order)
    {
        Orders.Add(order);
    }
    public void DisplayAllOrders()
    {
        foreach (Order order in Orders)
        {
            Console.WriteLine(order.ToString());
        }
    }
    public bool RemoveOrder(Order order)
    {
        bool removed = Orders.Remove(order);
        return removed;
        // return Orders.Remove(order);
    }

}
