using ShopApplication_Models.ViewModels;

namespace ShopApplication_Models.ApiModels;

public class CheckoutRequest
{
    public ProductUserVM ProductUserVM { get; set; }
    public string PaymentNonce { get; set; }
}