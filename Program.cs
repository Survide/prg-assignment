using System.Globalization;
using prg_asg;

Dictionary<string, Restaurant> restaurants = [];
List<FoodItem> foodItems = [];
Dictionary<string, Menu> menus = []; // restaurantId: menu
Dictionary<string, Customer> customers = []; // email: customer
Dictionary<string, Order> orders = []; // orderId: order
List<Order> refundStack = [];

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

        //  FIXME:: Did not pass in Orders as Customers have to be loaded b4 orders
        Customer c = new(email, name);
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
    Console.WriteLine($"{orders.Count} orders loaded!");
}

void ListRestaurantsAndMenu()
{
    Console.WriteLine("All Restaurants and Menu Items");
    Console.WriteLine("==============================");

    foreach (Restaurant restaurant in restaurants.Values)
    {
        Console.WriteLine($"Restaurant: {restaurant.RestaurantName} ({restaurant.RestaurantId})");

        // FIXME: Assume only got 1 menu
        foreach (FoodItem item in restaurant.Menus[0].FoodItems)
        {
            Console.WriteLine($"  - {item.ItemName}: {item.ItemDesc} - ${item.ItemPrice:f2}");
        }
        Console.WriteLine();
    }
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
    // prompt the user to enter Customer Email, Restaurant ID, Delivery Date/Time, Delivery Address
    Console.WriteLine("Create New Order");
    Console.WriteLine("==============================");

    // get inputs
    string email = Helper.GetValidInput("Enter Customer Email: ", "Invalid email entered.", s => customers.ContainsKey(s));
    string restaurantId = Helper.GetValidInput("Enter Restaurant ID: ", "Invalid Restaurant ID entered.", s => restaurants.ContainsKey(s));
    string date = Helper.GetValidInput("Enter Delivery Date (dd/mm/yyyy): ", "Invalid date entered.", s => DateTime.TryParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
    string time = Helper.GetValidInput("Enter Delivery Time (hh:mm): ", "Invalid time entered.", s => DateTime.TryParseExact(s, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
    string address = Helper.GetValidInput("Enter Delivery Address: ", "Invalid address entered.");

    DateTime dateTime = DateTime.ParseExact(date + " " + time, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
    Restaurant thisRest = restaurants[restaurantId];
    Customer thisCust = customers[email];

    // display available FoodItems
    Console.WriteLine("\nAvailable Food Items: ");
    // FIXME: Assume only got 1 menu
    int itemNumber = 1;
    foreach (FoodItem item in thisRest.Menus[0].FoodItems)
    {
        Console.WriteLine($"{itemNumber}. {item.ItemName} - ${item.ItemPrice:f2}");
        itemNumber++;
    }

    // allow the user to select multiple items and quantity
    List<OrderedFoodItem> orderItems = [];
    List<string> itemsParsed = []; // for csv storing

    int itemsCount = thisRest.Menus[0].FoodItems.Count;
    itemNumber = -1;
    while (itemNumber != 0)
    {
        itemNumber = int.Parse(Helper.GetValidInput("Enter item number (0 to finish): ", "Invalid item number entered", s => int.TryParse(s, out int val) && val >= 0 && val <= itemsCount));

        if (itemNumber == 0) break;

        int quantity = int.Parse(Helper.GetValidInput("Enter quantity: ", "Invalid quantity entered", s => int.TryParse(s, out int val) && val > 0));

        // create new OrderedFoodItems
        // FIXME: Assume only got 1 menu 
        FoodItem foodItem = thisRest.Menus[0].FoodItems[itemNumber - 1];
        OrderedFoodItem orderedFoodItem = new(foodItem, quantity);
        orderItems.Add(orderedFoodItem);

        // for csv storing
        itemsParsed.Add($"{foodItem.ItemName},{quantity}");
    }

    // apply special requests 
    string ifReq = Helper.GetValidInput("Add special request? [Y/N]: ", "Invalid input.", s => s.ToUpper() == "Y" || s.ToUpper() == "N").ToUpper();
    if (ifReq == "Y")
    {
        // FIXME: Not sure what request is for
        string request = Helper.GetValidInput("Enter request: ", "Request cannot be empty.");
    }

    // create new Order 
    Order newOrder = new()
    {
        DeliveryAddress = address,
        DeliveryDateTime = dateTime,
        OrderedFoodItems = orderItems,
        OrderDateTime = DateTime.Now,

        // For linking 
        FromRestaurant = thisRest,
        FromCustomer = thisCust,
    };
    double orderTotal = newOrder.CalculateOrderTotal();

    // calculate order total
    Console.WriteLine($"\nOrder Total: ${orderTotal:f2} + $5.00 (delivery) = ${orderTotal + 5:f2}");

    string ifPayment = Helper.GetValidInput("Proceed to payment? [Y/N]: ", "Invalid input.", s => s.Equals("Y", StringComparison.OrdinalIgnoreCase) || s.Equals("N", StringComparison.OrdinalIgnoreCase)).ToUpper();
    if (ifPayment == "N") return;

    // prompt user for payment method 
    string[] validOptions = ["CC", "PP", "CD"];

    string paymentMethod = Helper.GetValidInput("\nPayment method: [CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ", "Invalid payment method entered.", s => validOptions.Contains(s, StringComparer.OrdinalIgnoreCase));
    newOrder.OrderPaymentMethod = paymentMethod;

    // update status 
    newOrder.OrderStatus = "Pending";

    // assign new order id 
    newOrder.OrderId = 1000 + orders.Count + 1;
    orders[newOrder.OrderId.ToString()] = newOrder;

    thisRest.Orders.Enqueue(newOrder);
    thisCust.AddOrder(newOrder);
    orders[newOrder.OrderId.ToString()] = newOrder;

    // create csv item
    string orderStr = $"{newOrder.OrderId},{newOrder.FromCustomer.EmailAddress},{newOrder.FromRestaurant.RestaurantId},{date},{time},{address},{newOrder.OrderDateTime:dd/MM/yyyy HH:mm},{orderTotal},{newOrder.OrderStatus},\"{string.Join("|", itemsParsed)}\"";

    // append order to orders.csv 
    File.AppendAllText("data/orders.csv", orderStr);

    Helper.PrintColour(ConsoleColor.Green, $"\nOrder {newOrder.OrderId} created successfully! Status: {newOrder.OrderStatus}");
}

void ProcessOrder()
{
    Dictionary<string, string> errorMessageDict = new Dictionary<string, string>
    {
      {"C", "Pending"},
      {"R", "Pending"},
      {"S", "Cancelled"},
      {"D", "Preparing"},
    };
    while (true)
    {
        Console.WriteLine("Process Order");
        Console.WriteLine("=============");

        Console.Write("Enter Restaurant ID: ");
        string? restaurantId = Console.ReadLine();
        if (restaurantId == null)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Invalid Restaurand ID");
            Console.ResetColor();
            continue;
        }
        Console.WriteLine();
        if (!restaurants.ContainsKey(restaurantId))
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Invalid Restaurant ID");
            Console.ResetColor();
            continue;
        }
        int orderCounter = 1;
        while (orderCounter < restaurants[restaurantId].Orders.Count)
        {
            Order order = restaurants[restaurantId].Orders.ElementAt(orderCounter);

            // Skips if don't have Pending/Cancelled/Preparing
            if (!errorMessageDict.ContainsValue(order.OrderStatus))
            {
                orderCounter++;
                continue;
            }

            Console.WriteLine($"Order {order.OrderId}");
            Console.WriteLine($"Customer {order.FromCustomer.CustomerName}");
            Console.WriteLine("Ordered Items: ");
            int foodItemCounter = 1;
            foreach (OrderedFoodItem orderedFoodItem in order.OrderedFoodItems)
            {
                Console.WriteLine($"{foodItemCounter}. {orderedFoodItem.ItemName} - {orderedFoodItem.QtyOrdered}");
                foodItemCounter++;
            }
            Console.WriteLine($"Delivery date/time: {order.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm")}");
            Console.WriteLine($"Total Amount: {order.OrderTotal.ToString("F2")}");
            Console.WriteLine($"Order Status: {order.OrderStatus}");
            Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");

            string? option = Console.ReadLine();
            if (option == null || !errorMessageDict.ContainsKey(option))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Invalid option");
                Console.ResetColor();
            }
            else
            {
                // Checks for corresponding status. Checks this first to prevent nested ifs 
                if (order.OrderStatus != errorMessageDict[option])
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(
                        $"Order {order.OrderId} does not have status {errorMessageDict[option]}. Unable to proceed");
                    Console.ResetColor();
                    Console.WriteLine();
                    continue;
                }
                if (option == "C")
                {
                    order.OrderStatus = "Preparing";
                    Console.WriteLine($"Order {order.OrderId} confirmed. Status: Preparing");
                }
                else if (option == "R")
                {
                    order.OrderStatus = "Rejected";
                    refundStack.Add(order);
                    Console.WriteLine(
                        $"Order {order.OrderId} rejected. Status: Rejected. ${order.OrderTotal} has been refunded");
                }
                else if (option == "S")
                {
                    refundStack.Add(order);
                    Console.WriteLine($"Order {order.OrderId} skipped. Status: {order.OrderStatus}");
                }
                else if (option == "D")
                {
                    order.OrderStatus = "Delivered";
                    Console.WriteLine($"Order {order.OrderId} delivered. Status: Delivered");
                }
                orderCounter++;
            }
            Console.WriteLine();
        }
    }

}

void ModifyOrder()
{
    Console.WriteLine("Modify Order");
    Console.WriteLine("============");

    string email = Helper.GetValidInput("Enter Customer Email: ", "Invalid email entered.", s => customers.ContainsKey(s));
    Customer thisCust = customers[email];

    // display all orders that are pending for this customer
    Console.WriteLine("Pending Orders: ");
    foreach (Order order in thisCust.Orders)
    {
        if (order.OrderStatus == "Pending")
        {
            // FIXME: duplicate orderIds  
            Console.WriteLine(order.OrderId);
        }
    }
    string orderId = Helper.GetValidInput("\nEnter Order ID: ", "Invalid Order Id entered.", s => orders.ContainsKey(s) && orders[s].FromCustomer.EmailAddress == email);

    // display order items 
    Order thisOrder = orders[orderId];
    thisOrder.DisplayOrderedFoodItems();

    // display delivery info 
    Console.WriteLine("Address: ");
    Console.WriteLine(thisOrder.DeliveryAddress);
    Console.WriteLine("Delivery Date/Time: ");
    Console.WriteLine(thisOrder.DeliveryDateTime);

    // Console.WriteLine("Modify: [1] Items [2] Address [3] Delivery Time: ");
    string option = Helper.GetValidInput("\nModify: [1] Items [2] Address [3] Delivery Time: ", "Invalid option entered.", s => s == "1" || s == "2" || s == "3");

    if (option == "1")
    {
        // items 
        // FIXME: Assume that modify items only modifies quantity 
        List<OrderedFoodItem> orderedFoodItems = thisOrder.OrderedFoodItems;
        int itemNumber = int.Parse(Helper.GetValidInput("Enter order item number to modify: ", "Invalid item number entered.", s => int.TryParse(s, out int val) && val > 0 && val <= orderedFoodItems.Count));
        itemNumber--;

        int newQty = int.Parse(Helper.GetValidInput("Enter new quantity: ", "Invalid quantity entered", s => int.TryParse(s, out int val) && val >= 0));
        int currentQty = orderedFoodItems[itemNumber].QtyOrdered;
        double priceDiff = orderedFoodItems[itemNumber].ItemPrice * Math.Abs(newQty - currentQty);

        // prompt the user to pay if there is an increase in the order total 
        if (newQty > currentQty)
        {
            string ifPayment = Helper.GetValidInput("Proceed to payment? [Y/N]: ", "Invalid input.", s => s.Equals("Y", StringComparison.OrdinalIgnoreCase) || s.Equals("N", StringComparison.OrdinalIgnoreCase)).ToUpper();
            if (ifPayment == "N") return;

            // prompt user for payment method 
            string[] validOptions = ["CC", "PP", "CD"];

            string paymentMethod = Helper.GetValidInput("\nPayment method: [CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ", "Invalid payment method entered.", s => validOptions.Contains(s, StringComparer.OrdinalIgnoreCase));
            thisOrder.OrderPaymentMethod = paymentMethod;
        }
        else if (newQty < currentQty)
        {
            // FIXME: Assume that there will be a refund 
            Helper.PrintColour(ConsoleColor.Green, $"${priceDiff:f2} will be refunded.");
        }

        // remove this order if the quantity is 0 
        if (newQty == 0)
        {
            thisOrder.RemoveOrderedFoodItem(orderedFoodItems[itemNumber]);
        }
        else
        {
            thisOrder.OrderedFoodItems[itemNumber].QtyOrdered = newQty;
        }

        Helper.PrintColour(ConsoleColor.Green, $"Order {thisOrder.OrderId} updated. Updated food items: ", () => thisOrder.DisplayOrderedFoodItems());

    }
    else if (option == "2")
    {
        // address 
        string address = Helper.GetValidInput("Enter New Delivery Address: ", "Invalid address entered.");
        thisOrder.DeliveryAddress = address;

        // confirmation message
        Helper.PrintColour(ConsoleColor.Green, $"Order {thisOrder.OrderId} updated. New Address: {address}");
    }
    else
    {
        // delivery time 
        string time = Helper.GetValidInput("Enter Delivery Time (hh:mm): ", "Invalid time entered.", s => DateTime.TryParseExact(s, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
        string[] timeSplit = time.Split(":");
        (int hours, int minutes) = (int.Parse(timeSplit[0]), int.Parse(timeSplit[1]));

        thisOrder.DeliveryDateTime = thisOrder.DeliveryDateTime.Date + new TimeSpan(hours, minutes, 0);

        // confirmation message
        Helper.PrintColour(ConsoleColor.Green, $"Order {thisOrder.OrderId} updated. New Delivery Time: {time}");
    }

}

void DeleteOrder()
{
    List<string> validInputs = ["Y", "N"];
    while (true)
    {
        Console.WriteLine("Delete Order");
        Console.WriteLine("============");
        Console.Write("Enter Customer Email: ");
        string? email = Console.ReadLine();
        if (email == null || !customers.ContainsKey(email))
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Invalid input / Email not found");
            Console.ResetColor();
            continue;
        }
        Console.WriteLine("Pending Orders:");
        foreach (Order customerOrder in customers[email].Orders)
        {
            if (customerOrder.OrderStatus == "Pending")
            {
                Console.WriteLine(customerOrder.OrderId);
            }
        }
        Console.Write("Enter Order ID: ");
        string? orderId = Console.ReadLine();
        if (orderId == null || !orders.ContainsKey(orderId))
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Invalid order ID.");
            Console.ResetColor();
            continue;
        }
        Order order = orders[orderId];
        Console.WriteLine($"Order {order.OrderId}");
        Console.WriteLine($"Customer {order.FromCustomer.CustomerName}");
        Console.WriteLine("Ordered Items: ");
        int foodItemCounter = 1;
        foreach (OrderedFoodItem orderedFoodItem in order.OrderedFoodItems)
        {
            Console.WriteLine($"{foodItemCounter}. {orderedFoodItem.ItemName} - {orderedFoodItem.QtyOrdered}");
            foodItemCounter++;
        }
        Console.WriteLine($"Delivery date/time: {order.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm")}");
        Console.WriteLine($"Total Amount: {order.OrderTotal.ToString("F2")}");
        Console.WriteLine($"Order Status: {order.OrderStatus}");
        Console.Write("Confirm deletion? [Y/N]: ");
        string? confirm = Console.ReadLine();
        if (confirm == null || !validInputs.Contains(confirm))
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Invalid input.");
            Console.ResetColor();
            Console.WriteLine();
            continue;
        }
        if (confirm == "Y")
        {
            order.OrderStatus = "Cancelled";
            refundStack.Add(order);
            Console.WriteLine(
                $"Order {order.OrderId} cancelled. Refund of ${order.OrderTotal.ToString("F2")} processed.");
        }
    }
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
            Console.WriteLine();
        }
        catch (FormatException)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Invalid input.");
            Console.ResetColor();
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
