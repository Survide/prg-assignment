namespace prg_asg;

public class Menu
{
    public string MenuId { get; set; }
    public string MenuName { get; set; }
    public List<FoodItem> FoodItems { get; set; }

    public Menu(string menuId, string menuName)
    {
        MenuId = menuId;
        MenuName = menuName;
        FoodItems = new List<FoodItem>();
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
