namespace prg_asg;

public class FoodItem
{
    public string ItemName { get; set; }
    public string ItemDesc { get; set; }
    public double ItemPrice { get; set; }
    public string Customise { get; set; }

    public FoodItem(string itemName, string itemDesc, double itemPrice, string customise)
    {
        ItemName = itemName;
        ItemDesc = itemDesc;
        ItemPrice = itemPrice;
        Customise = customise;
    }

    public override string ToString()
    {
        return
          "ItemName: " + ItemName +
          " ItemDesc: " + ItemDesc +
          " ItemPrice: " + ItemPrice +
          " Customise: " + Customise;
    }

}
