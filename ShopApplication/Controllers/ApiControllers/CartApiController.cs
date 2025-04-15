using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models.ApiModels;
using ShopApplication_Utility.BrainTree;

namespace ShopApplication.Controllers.ApiControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartApiController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IBrainTreeGate _brainTreeService;

    public CartApiController(ICartService cartService, IEmailSenderService emailSenderService, IBrainTreeGate brainTreeService)
    {
        _cartService = cartService;
        _emailSenderService = emailSenderService;
        _brainTreeService = brainTreeService;
    }

    [HttpGet("client-token")]
    public IActionResult GetClientToken()
    {
        try
        {
            var clientToken = _brainTreeService.GetClientBrainTreeToken();
            return Ok(new { clientToken });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error generating client token: {ex.Message}");
        }
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest checkoutRequest)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized("User is not authenticated");
        }

        try
        {
            var orderId = await _cartService.CreateOrderAsync(checkoutRequest.ProductUserVM, userId);

            var success = await _cartService.ProcessPaymentAsync(orderId, checkoutRequest.PaymentNonce);
            if (success)
            {
                await _emailSenderService.SendEmailAsync(orderId, checkoutRequest.ProductUserVM);
                
                return Ok(new { message = "Payment Successful", orderId });
            }

            return BadRequest("Payment failed");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error during checkout: {ex.Message}");
        }
    }

    [HttpPut("update")]
    public IActionResult UpdateCart([FromBody] CartUpdateRequest cartUpdateRequest)
    {
        try
        {
            _cartService.UpdateCart(cartUpdateRequest.Products);
            return Ok(new { message = "Cart updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error updating cart: {ex.Message}");
        }
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetCartItems()
    {
        try
        {
            var cartItems = await _cartService.GetCartProductsAsync();
            return Ok(cartItems);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error fetching cart items: {ex.Message}");
        }
    }

    [HttpDelete("remove/{id}")]
    public IActionResult RemoveFromCart(int id)
    {
        try
        {
            _cartService.RemoveFromCart(id);
            return Ok(new { message = "Item removed from cart successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error removing item from cart: {ex.Message}");
        }
    }

    [HttpDelete("clear")]
    public IActionResult ClearCart()
    {
        try
        {
            _cartService.ClearCart();
            return Ok(new { message = "Cart cleared successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error clearing cart: {ex.Message}");
        }
    }
}