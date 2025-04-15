using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication_Models.ViewModels;
using ShopApplication_Utility;

namespace ShopApplication.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHomeService _homeService;

    public HomeController(ILogger<HomeController> logger, IHomeService homeService)
    {
        _logger = logger;
        _homeService = homeService;
    }

    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Index action called at {Time}", DateTime.UtcNow);
        var homeVm = await _homeService.GetHomeViewModelAsync();
        return View(homeVm);
    }

    public async Task<IActionResult> Details(int id)
    {
        _logger.LogInformation("Details action called with id {ProductId} at {Time}", id, DateTime.UtcNow);
        var detailsVm = await _homeService.GetDetailsViewModelAsync(id);
        return View(detailsVm);
    }
    [HttpPost]
    [ActionName("Details")]
    public IActionResult Details(int id, DetailsVM detailsVm)
    {
        if (detailsVm.Product != null)
        {
            _logger.LogInformation("DetailsPost action called with id {ProductId} and quantity {Quantity} at {Time}",
                id, detailsVm.Product.TempUnits, DateTime.UtcNow);

            try
            {
                _homeService.AddToCart(id, detailsVm.Product.TempUnits);
                TempData[Constants.Success] = "Product is added!";
                _logger.LogInformation("Product with id {ProductId} successfully added to cart at {Time}", id,
                    DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding product with id {ProductId} to cart at {Time}", id,
                    DateTime.UtcNow);
                TempData[Constants.Error] = "Failed to add product to cart.";
            }
        }

        return RedirectToAction(nameof(System.Index));
    }

    public IActionResult RemoveFromCart(int id)
    {
        _logger.LogInformation("RemoveFromCart action called with id {ProductId} at {Time}", id, DateTime.UtcNow);
        try
        {
            _homeService.RemoveFromCart(id);
            _logger.LogInformation("Product with id {ProductId} removed from cart at {Time}", id, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing product with id {ProductId} from cart at {Time}", id, DateTime.UtcNow);
        }
        return RedirectToAction(nameof(System.Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("Error occurred with RequestId {RequestId} at {Time}", requestId, DateTime.UtcNow);
        return View(new ErrorViewModel { RequestId = requestId });
    }
}