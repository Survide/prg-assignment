
using System.Globalization;
using prg_asg;

Dictionary<string, Restaurant> restaurants = [];
List<FoodItem> foodItems = [];
Dictionary<string, Menu> menus = []; // restaurantId: menu
Dictionary<string, Customer> customers = []; // email: customer
Dictionary<string, Order> orders = []; // orderId: order

void LoadRestaurants()
{
    try
    {
        string[] records = [.. File.ReadAllLines("data/restaurants.csv").Skip(1)];
        foreach (string record in records)
        {
            string[] details = record.Split(",");
            restaurants.Add(details[0], new Restaurant(details[0], details[1], details[2]));
        }
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine("Restaurants file not found!");
        Console.WriteLine($"Error message {ex.Message}");
    }
    finally
    {
        Console.WriteLine($"{restaurants.Count} restaurants loaded!");
    }
}

void LoadFoodItems()
{
    string[] records = [];
    try
    {
        records = [.. File.ReadAllLines("data/fooditems.csv").Skip(1)];
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine("FoodItems file not found!");
        Console.WriteLine($"Error message {ex.Message}");
        return;
    }
    foreach (string record in records)
    {
        FoodItem foodItem;
        string restaurantId;
        string name;
        string description;
        string stringPrice;
        double price;
        // theres a record that has "" in description.
        if (record.Contains('"'))
        {
            string[] splitDetails = record.Split('"');
            string[] restauranntIdAndName = splitDetails[0].Substring(0, splitDetails[0].Length - 1).Split(",");
            restaurantId = restauranntIdAndName[0];
            name = restauranntIdAndName[1];
            description = splitDetails[1];
            stringPrice = splitDetails[2].Substring(1);
        }
        else
        {
            string[] details = record.Split(",");
            restaurantId = details[0];
            name = details[1];
            description = details[2];
            stringPrice = details[3];
        }
        if (!double.TryParse(stringPrice, out price))
        {
            Console.WriteLine($"Could not parse price to double in line: {record}");
            continue;
        }
        foodItem = new FoodItem(name, description, price, "");
        foodItems.Add(foodItem);
        if (menus.ContainsKey(restaurantId))
        {
            menus[restaurantId].FoodItems.Add(foodItem);
        }
        else
        {
            menus.Add(restaurantId, new Menu(restaurantId, "Main Menu", [foodItem]));
        }
    }
    foreach (KeyValuePair<string, Menu> kvp in menus)
    {
        string restaurantId = kvp.Key;
        restaurants[restaurantId].Menus.Add(kvp.Value);
    }
    Console.WriteLine($"{foodItems.Count} food items loaded!");
}

void LoadCustomers()
{
    string[] records = [];
    try
    {
        records = [.. File.ReadAllLines("data/customers.csv").Skip(1)];
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine("Customers file not found!");
        Console.WriteLine($"Error message {ex.Message}");
    }
    foreach (string record in records)
    {
        string[] split = record.Split(",");
        (string name, string email) = (split[0], split[1]);

        //  FIXME:: Passed in empty [] for orders since Customers have to be loaded b4 orders
        Customer c = new(email, name, []);
        customers[email] = c;
    }

    Console.WriteLine($"{customers.Count} customers loaded!");
}

void LoadOrders()
{
    string[] records = [];
    try
    {
        records = [.. File.ReadAllLines("data/orders.csv").Skip(1)];
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine("Orders file not found!");
        Console.WriteLine($"Error message {ex.Message}");
    }

    foreach (string record in records)
    {
        // get the food items 
        int startOfItems = record.IndexOf('"');
        string[] d = record[0..startOfItems].Split(",");
        string itemsRaw = record[startOfItems..^0];

        (string orderId, string customerEmail, string restaurantId, string deliveryDate, string deliveryTime, string deliveryAddress, string createdDateTime, string totalAmount, string status) = (d[0], d[1], d[2], d[3], d[4], d[5], d[6], d[7], d[8]);

        List<OrderedFoodItem> foodItems = [];
        Restaurant thisRest = restaurants[restaurantId];
        Customer thisCust = customers[customerEmail];

        string[] itemsParsed = itemsRaw[1..^1].Split("|");
        for (int i = 0; i < itemsParsed.Length; i++)
        {
            string[] s = itemsParsed[i].Split(",");
            string name = s[0];
            int qty = s.Length == 1 ? 1 : int.Parse(s[1].Trim()); // by default qty = 1 

            // find the foodItem in restaurant's menu 
            bool isFound = false;
            foreach (Menu menu in thisRest.Menus)
            {
                foreach (FoodItem item in menu.FoodItems)
                {
                    // check for this item 
                    if (item.ItemName == name)
                    {
                        isFound = true;
                        OrderedFoodItem f = new(item, qty);
                        foodItems.Add(f);
                        break;
                    }
                }
                if (isFound) break;
            }

            Order newOrder = new Order(
                int.Parse(orderId),
                DateTime.ParseExact(createdDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                DateTime.ParseExact(deliveryDate + " " + deliveryTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                deliveryAddress,
                double.Parse(totalAmount),
                status,
                thisCust,
                thisRest,
                null,
                foodItems
            );

            // place them into the Restaurant’s Order Queue and the Customer’s Order List
            thisRest.Orders.Enqueue(newOrder);
            thisCust.AddOrder(newOrder);

            orders[orderId] = newOrder;
        }
    }
    Console.WriteLine($"{orders.Count} orders loaded!");
}

void ListRestaurantsAndMenu()
{
}

void ListAllOrders()
{
    Console.WriteLine("All Orders");
    Console.WriteLine("==========");
    Console.WriteLine(
    $"{"Order ID",-10}" +
    $"{"Customer",-15}" +
    $"{"Restaurant",-20}" +
    $"{"Delivery Date/Time",-20}" +
    $"{"Amount",-8}" +
    $"{"Status"}"
    );
    Console.WriteLine(
    $"{"________",-10}" +
    $"{"________",-15}" +
    $"{"__________",-20}" +
    $"{"__________________",-20}" +
    $"{"______",-8}" +
    $"{"______"}"
    );
    foreach (Order order in orders.Values)
    {
        Console.WriteLine(
        $"{order.OrderId,-10}" +
        $"{order.FromCustomer.CustomerName,-15}" +
        $"{order.FromRestaurant.RestaurantName,-20}" +
        $"{order.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm"),-20}" + // need format
        $"${order.OrderTotal.ToString("F2"),-7}" +
        order.OrderStatus
        );
    }
}

void CreateOrder()
{
}

void ProcessOrder()
{
}

void ModifyOrder()
{
}

void DeleteOrder()
{
}

void MainMenu()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("===== Gruberoo Food Delivery System =====");
        Console.WriteLine("1. List all restaurants and menu items");
        Console.WriteLine("2. List all orders");
        Console.WriteLine("3. Create a new order");
        Console.WriteLine("4. Process an order");
        Console.WriteLine("5. Modify an existing order");
        Console.WriteLine("6. Delete an existing order");
        Console.WriteLine("0. Exit");
        Console.Write("Enter your choice: ");

        int option = -1;
        try
        {
            option = Convert.ToInt16(Console.ReadLine());
        }
        catch (FormatException ex)
        {
            Console.WriteLine("Could not convert to Int16");
            Console.WriteLine($"Error message: {ex.Message}");
        }
        if (option == 0)
        {
            break;
        }
        else if (option == 1)
        {
            ListRestaurantsAndMenu();
        }
        else if (option == 2)
        {
            ListAllOrders();
        }
        else if (option == 3)
        {
            CreateOrder();
        }
        else if (option == 4)
        {
            ProcessOrder();
        }
        else if (option == 5)
        {
            ModifyOrder();
        }
        else if (option == 6)
        {
            DeleteOrder();
        }
    }

}

void InitializeGruberoo()
{
    Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
    LoadRestaurants();
    LoadFoodItems();
    LoadCustomers();
    LoadOrders();
    MainMenu();
}

InitializeGruberoo();
