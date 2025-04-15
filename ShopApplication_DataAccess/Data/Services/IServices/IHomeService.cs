using ShopApplication_Models.ViewModels;

namespace ShopApplication_DataAccess.Data.Services.IServices;

public interface IHomeService
{
    public Task<HomeVM> GetHomeViewModelAsync();
    public Task<DetailsVM> GetDetailsViewModelAsync(int id);
    public void AddToCart(int id, int quantity);
    public void RemoveFromCart(int id);
}