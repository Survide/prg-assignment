namespace prg_asg;

public class SpecialOffer
{
    public string OfferCode { get; set; }
    public string OfferDesc { get; set; }
    public double Discount { get; set; }

    public List<Order> Orders { get; set; }

    public SpecialOffer(string offerCode, string offerDesc, double discount, List<Order> orders)
    {
        OfferCode = offerCode;
        OfferDesc = offerDesc;
        Discount = discount;

        Orders = orders;
    }

    public override string ToString()
    {
        return
          "OfferCode: " + OfferCode +
          " OfferDesc: " + OfferDesc +
          " Discount: " + Discount;
    }

}
