
using prg_asg;

List<Restaurant> restaurants = [];
List<FoodItem> foodItems = [];

void LoadRestaurants()
{
    try
    {
        string[] records = File.ReadAllLines("data/restaurants.csv").Skip(1).ToArray();
        foreach (string record in records)
        {
            string[] details = record.Split(",");
            restaurants.Add(new Restaurant(details[0], details[1], details[2]));
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
    try
    {
        string[] records = File.ReadAllLines("data/fooditems.csv").Skip(1).ToArray();
        foreach (string record in records)
        {
            // theres a record that has "" in description.
            if (record.Contains('"'))
            {
                string[] splitDetails = record.Split('"');
                string name = splitDetails[0].Substring(0, splitDetails[0].Length - 1).Split(",")[1];
                string description = splitDetails[1];
                string stringPrice = splitDetails[2].Substring(1);
                if (!double.TryParse(stringPrice, out double splitPrice))
                {
                    Console.WriteLine($"Could not parse price to double in line: {record}");
                    continue;
                }
                foodItems.Add(new FoodItem(name, description, splitPrice, ""));
                continue;
            }
            string[] details = record.Split(",");
            if (!double.TryParse(details[3], out double price))
            {
                Console.WriteLine($"Could not parse price to double in line: {record}");
                continue;
            }
            foodItems.Add(new FoodItem(details[1], details[2], price, ""));
        }
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine("FoodItems file not found!");
        Console.WriteLine($"Error message {ex.Message}");
    }
    finally
    {
        Console.WriteLine($"{foodItems.Count()} food items loaded!");
    }
}

void LoadCustomers()
{
}

void LoadOrders()
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

        int option = 7;
        try
        {
            option = Convert.ToInt16(Console.ReadLine());
        }
        catch (FormatException ex)
        {
            Console.WriteLine("Could not convert to Int16");
            Console.WriteLine($"Error message: {ex.Message}");
        }
    }

}

void InitializeGruberoo()
{
    Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
    LoadRestaurants();
    LoadFoodItems();
}

InitializeGruberoo();
