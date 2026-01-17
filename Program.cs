
using System.Globalization;
using prg_asg;

Dictionary<string, Restaurant> restaurants = [];
List<FoodItem> foodItems = [];
List<Order> orders = [];
Dictionary<Customer, Order> customerOrders = []; // stores the customer: their order

void LoadRestaurants()
{
    try
    {
        string[] records = File.ReadAllLines("data/restaurants.csv").Skip(1).ToArray();
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
        Console.WriteLine($"{restaurants.Count()} restaurants loaded!");
    }
}

void LoadFoodItems()
{
    string[] records = [];
    Dictionary<string, Menu> menus = new Dictionary<string, Menu>();
    try
    {
        records = File.ReadAllLines("data/fooditems.csv").Skip(1).ToArray();
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
        // theres a record that has "" in description.
        if (record.Contains('"'))
        {
            string[] splitDetails = record.Split('"');
            string[] restauranntIdAndName = splitDetails[0].Substring(0, splitDetails[0].Length - 1).Split(",");
            string restaurantId = restauranntIdAndName[0];
            string name = restauranntIdAndName[1];
            string description = splitDetails[1];
            string stringPrice = splitDetails[2].Substring(1);
            if (!double.TryParse(stringPrice, out double splitPrice))
            {
                Console.WriteLine($"Could not parse price to double in line: {record}");
                continue;
            }
            foodItem = new FoodItem(name, description, splitPrice, "");
            foodItems.Add(foodItem);
            if (menus.ContainsKey(restaurantId))
            {
                menus[restaurantId].FoodItems.Add(foodItem);
            }
            else
            {
                menus.Add(restaurantId, new Menu(restaurantId, "Main Menu", [foodItem]));
            }
            continue;
        }
        string[] details = record.Split(",");
        if (!double.TryParse(details[3], out double price))
        {
            Console.WriteLine($"Could not parse price to double in line: {record}");
            continue;
        }
        foodItem = new FoodItem(details[1], details[2], price, "");
        foodItems.Add(foodItem);
        if (menus.ContainsKey(details[0]))
        {
            menus[details[0]].FoodItems.Add(foodItem);
        }
        else
        {
            menus.Add(details[0], new Menu(details[0], "Main Menu", [foodItem]));
        }
    }
    Console.WriteLine($"{foodItems.Count()} food items loaded!");
}

void LoadCustomers()
{
}

void LoadOrders()
{
    try
    {
        string[] records = File.ReadAllLines("data/orders.csv").Skip(1).ToArray();
        foreach (string record in records)
        {
            string[] d = record.Split(",");

            (string orderId, string customerEmail, string restaurantId, string deliveryDate, string deliveryTime, string deliveryAddress, string createdDateTime, string totalAmount, string status, string itemsRaw) = (d[0], d[1], d[2], d[3], d[4], d[5], d[6], d[7], d[8], d[9]);

            List<OrderedFoodItem> foodItems = [];
            string[] itemsParsed = itemsRaw.Split("|");
            Restaurant res = restaurants[restaurantId];
            // TODO: get food item based on Restaurant.Menu.FoodItem
            for (int i = 0; i < itemsParsed.Length; i++)
            {
                string[] s = itemsParsed[i].Split(",");
                (string name, int qty) = (s[0], int.Parse(s[1]));
                // OrderedFoodItem item = new(name, "", );
            }

            // OrderId,CustomerEmail,RestaurantId,DeliveryDate,DeliveryTime,DeliveryAddress,CreatedDateTime,TotalAmount,Status,Items
            Order newOrder = new Order(int.Parse(orderId), DateTime.ParseExact(createdDateTime, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture), DateTime.ParseExact(deliveryDate + " " + deliveryTime, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture), deliveryAddress, double.Parse(totalAmount), status, null, foodItems);
        }
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine("Restaurants file not found!");
        Console.WriteLine($"Error message {ex.Message}");
    }
    finally
    {
        Console.WriteLine($"{restaurants.Count()} restaurants loaded!");
    }

}

void ListRestaurantsAndMenu()
{
}

void ListAllOrders()
{
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
    // LoadCustomers(); Uncomment when done
    // LoadOrders();
}

InitializeGruberoo();
