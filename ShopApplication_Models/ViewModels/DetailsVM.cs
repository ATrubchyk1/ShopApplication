namespace ShopApplication_Models.ViewModels;

public class DetailsVM
{
    public DetailsVM()
    {
        Product = new Product();
    }
    public Product? Product { get; set; }
    public bool ExistInCard { get; set; }
}