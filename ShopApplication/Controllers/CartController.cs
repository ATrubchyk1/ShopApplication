using System.Security.Claims;
using System.Text;
using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication_Models.ViewModels;
using ShopApplication_Utility;
using ShopApplication_Utility.BrainTree;

namespace ShopApplication.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IBrainTreeGate _brainTreeService;

    [BindProperty]
    public ProductUserVM ProductUserVM { get; set; }

    public CartController(
        ICartService cartService,
        IEmailSenderService emailSenderService,
        IBrainTreeGate brainTreeService)
    {
        _cartService = cartService;
        _emailSenderService = emailSenderService;
        _brainTreeService = brainTreeService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _cartService.GetCartProductsAsync());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Index")]
    public IActionResult IndexPost(IEnumerable<Product> products)
    {
        _cartService.UpdateCart(products);
        return RedirectToAction(nameof(Summary));
    }

    public async Task<IActionResult> Summary()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null)
        {
            var productUserVm = await _cartService.GetSummaryAsync(userId, User.IsInRole(Constants.AdminRole));
            ViewBag.ClientToken = _brainTreeService.GetClientBrainTreeToken();

            return View(productUserVm);
        }

        return NotFound();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Summary")]
    public async Task<IActionResult> SummaryPost(IFormCollection collection, ProductUserVM productUserVm)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null)
        {
            var orderId = await _cartService.CreateOrderAsync(productUserVm, userId);
            var success = await _cartService.ProcessPaymentAsync(orderId, collection["payment_method_nonce"]);
        
            if (success)
                TempData[Constants.Success] = "Payment Successful!";
            else
                TempData[Constants.Error] = "Payment Failed!";
        
            await _emailSenderService.SendEmailAsync(orderId, productUserVm);
            TempData[Constants.Success] = "Inquiry submitted!";
            return RedirectToAction("InquiryConfirmation", new { id = orderId });
        }

        return NotFound();
    }
    
    public async Task<IActionResult> InquiryConfirmation(int id)
    {
        var orderHeader = await _cartService.GetOrderByIdAsync(id);
        _cartService.ClearCart();
        return View(orderHeader);
    }

    public IActionResult Remove(int id)
    {
        _cartService.RemoveFromCart(id);
        return RedirectToAction(nameof(System.Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateCart(IEnumerable<Product> products)
    {
        _cartService.UpdateCart(products);
        return RedirectToAction(nameof(System.Index));
    }
    
    public IActionResult Clear()
    {
        _cartService.ClearCart();
        return RedirectToAction(nameof(System.Index));
    }
}