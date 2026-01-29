namespace prg_asg;

public class Restaurant
{
    public string RestaurantId { get; set; }
    public string RestaurantName { get; set; }
    public string RestaurantEmail { get; set; }
    public Queue<Order> Orders { get; set; }
    public List<Menu> Menus { get; set; }
    public List<SpecialOffer> SpecialOffers { get; set; }

    public Restaurant(
        string restaurantId,
        string restaurantName,
        string restaurantEmail
    )
    {
        RestaurantId = restaurantId;
        RestaurantName = restaurantName;
        RestaurantEmail = restaurantEmail;

        Orders = [];
        Menus = [new Menu("main", "main menu")];
        SpecialOffers = [new SpecialOffer("no", "default no discount", 0)];
    }

    public void DisplayOrders()
    {
        foreach (Order order in Orders)
        {
            Console.WriteLine(order.ToString());
        }
    }
    public void DisplaySpecialOffers()
    {
        foreach (SpecialOffer specialOffer in SpecialOffers)
        {
            Console.WriteLine(specialOffer.ToString());
        }
    }
    public void DisplayMenu()
    {
        foreach (Menu menu in Menus)
        {
            Console.WriteLine(menu.ToString());
        }
    }
    public void AddMenu(Menu menu)
    {
        Menus.Add(menu);
    }
    public bool RemoveMenu(Menu menu)
    {
        bool removed = Menus.Remove(menu);
        return removed;
    }

    public override string ToString()
    {
        return
          "RestaurantId: " + RestaurantId +
          " RestaurantName " + RestaurantName +
          " RestaurantEmail " + RestaurantEmail;
    }

}
