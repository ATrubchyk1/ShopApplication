namespace ShopApplication_Models.ApiModels;

public class CartUpdateRequest
{
    public IEnumerable<Product> Products { get; set; }
}