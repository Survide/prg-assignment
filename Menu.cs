namespace prg_asg;

public class Menu
{
    public string MenuId { get; set; }
    public string MenuName { get; set; }
    public List<FoodItem> FoodItems { get; set; }

    public Menu(string menuId, string menuName, List<FoodItem> foodItems)
    {
        MenuId = menuId;
        MenuName = menuName;

        FoodItems = foodItems;
    }

    public void AddFoodItem(FoodItem foodItem)
    {
        FoodItems.Add(foodItem);
    }
    public bool RemoveFoodItem(FoodItem foodItem)
    {
        bool removed = FoodItems.Remove(foodItem);
        return removed;
    }
    public void DisplayFoodItems()
    {
        foreach (FoodItem foodItem in FoodItems)
        {
            Console.WriteLine(foodItem.ToString());
        }
    }
    public override string ToString()
    {
        return "MenuId: " + MenuId + " MenuName: " + MenuName;
    }

}
