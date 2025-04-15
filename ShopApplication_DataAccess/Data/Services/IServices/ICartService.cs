using ShopApplication_Models;
using ShopApplication_Models.ViewModels;

namespace ShopApplication_DataAccess.Data.Services.IServices;

public interface ICartService
{
    Task<List<Product>> GetCartProductsAsync();
    void UpdateCart(IEnumerable<Product> products);
    void RemoveFromCart(int id);
    void ClearCart();
    Task<ProductUserVM> GetSummaryAsync(string userId, bool isAdmin);
    Task<int> CreateOrderAsync(ProductUserVM productUserVm, string userId);
    Task<bool> ProcessPaymentAsync(int orderId, string nonce);
    public Task<OrderHeader?> GetOrderByIdAsync(int? id);
}