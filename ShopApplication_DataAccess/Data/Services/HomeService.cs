using Microsoft.AspNetCore.Http;
using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication_Models.ViewModels;
using ShopApplication_Utility;

namespace ShopApplication_DataAccess.Data.Services;

public class HomeService : IHomeService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HomeService(IProductRepository productRepository, ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<HomeVM> GetHomeViewModelAsync()
    {
        return new HomeVM()
        {
            Products = await _productRepository.GetAllAsync(includes: "Category,ApplicationType"),
            Categories = await _categoryRepository.GetAllAsync()
        };
    }

    public async Task<DetailsVM> GetDetailsViewModelAsync(int id)
    {
        var shoppingCartList = _httpContextAccessor.HttpContext?.Session.Get<List<ShoppingCart>>(Constants.SessionCart) ?? new List<ShoppingCart>();

        var product = await _productRepository.FirstOrDefaultAsync(x => x.Id == id, includes: "Category,ApplicationType");

        return new DetailsVM()
        {
            Product = product,
            ExistInCard = shoppingCartList.Any(x => x.ProductId == id)
        };
    }

    public void AddToCart(int id, int quantity)
    {
        var shoppingCartList = _httpContextAccessor.HttpContext?.Session.Get<List<ShoppingCart>>(Constants.SessionCart) ?? new List<ShoppingCart>();

        shoppingCartList.Add(new ShoppingCart { ProductId = id, Units = quantity });

        _httpContextAccessor.HttpContext?.Session.Set(Constants.SessionCart, shoppingCartList);
    }

    public void RemoveFromCart(int id)
    {
        var shoppingCartList = _httpContextAccessor.HttpContext?.Session.Get<List<ShoppingCart>>(Constants.SessionCart) ?? new List<ShoppingCart>();

        var itemToRemove = shoppingCartList.FirstOrDefault(x => x.ProductId == id);
        if (itemToRemove != null)
        {
            shoppingCartList.Remove(itemToRemove);
        }

        _httpContextAccessor.HttpContext?.Session.Set(Constants.SessionCart, shoppingCartList);
    }
}