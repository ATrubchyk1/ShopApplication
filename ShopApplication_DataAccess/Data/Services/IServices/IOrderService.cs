using ShopApplication_Models;
using ShopApplication_Models.ViewModels;

namespace ShopApplication_DataAccess.Data.Services.IServices;

public interface IOrderService
{
    public Task<OrderVM> GetOrderDetailsAsync(int id);
    public Task StartProcessingOrderAsync(int orderId);
    public Task ShipOrderAsync(int orderId);
    public Task CancelOrderAsync(int orderId);
    public Task UpdateOrderDetailsAsync(OrderVM orderVM);
    public Task<IEnumerable<OrderHeader>> GetAllAsync();
}