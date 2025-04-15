namespace ShopApplication_Models.ViewModels;

public class ProductUserVM
{
    public ProductUserVM()
    {
        ProductsList = new List<Product>();
    }
    public ApplicationUser? ApplicationUser { get; set; }
    public IList<Product> ProductsList { get; set; }
}