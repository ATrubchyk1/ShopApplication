using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models.ViewModels;
using ShopApplication_Utility;

namespace ShopApplication.Controllers.ApiControllers;

[Authorize(Roles = Constants.AdminRole)]
[Route("api/[controller]")]
[ApiController]
public class OrderControllerApiController : ControllerBase
{
    private readonly IOrderService _orderService;
    
    public OrderControllerApiController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetOrders(string searchName = null, string searchEmail = null, string searchPhone = null, string status = null)
    {
        var orderListVm = new OrderListVM()
        {
            OrderHeaderList = await _orderService.GetAllAsync()
        };

        if (!string.IsNullOrEmpty(searchName))
        {
            orderListVm.OrderHeaderList = orderListVm.OrderHeaderList.Where(x => x.FullName.ToLower().Contains(searchName.ToLower()));
        }
        if (!string.IsNullOrEmpty(searchEmail))
        {
            orderListVm.OrderHeaderList = orderListVm.OrderHeaderList.Where(x => x.Email != null && x.Email.ToLower().Contains(searchEmail.ToLower()));
        }
        if (!string.IsNullOrEmpty(searchPhone))
        {
            orderListVm.OrderHeaderList = orderListVm.OrderHeaderList.Where(x => x.PhoneNumber != null && x.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
        }
        if (!string.IsNullOrEmpty(status) && status != "--Order Status--")
        {
            orderListVm.OrderHeaderList = orderListVm.OrderHeaderList.Where(x => x.OrderStatus.ToLower().Contains(status.ToLower()));
        }

        return Ok(orderListVm.OrderHeaderList);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderDetails(int id)
    {
        var orderDetailsVm = await _orderService.GetOrderDetailsAsync(id);
        if (orderDetailsVm == null)
        {
            return NotFound($"Order with ID {id} not found.");
        }
        return Ok(orderDetailsVm);
    }

    [HttpPost("{id}/start-processing")]
    public async Task<IActionResult> StartProcessingOrder(int id)
    {
        var order = await _orderService.GetOrderDetailsAsync(id);
        if (order == null)
        {
            return NotFound($"Order with ID {id} not found.");
        }

        await _orderService.StartProcessingOrderAsync(id);
        return Ok(new { message = "Order is now in progress." });
    }

    [HttpPost("{id}/ship")]
    public async Task<IActionResult> ShipOrder(int id)
    {
        var order = await _orderService.GetOrderDetailsAsync(id);
        if (order == null)
        {
            return NotFound($"Order with ID {id} not found.");
        }

        await _orderService.ShipOrderAsync(id);
        return Ok(new { message = "Order has been shipped successfully." });
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var order = await _orderService.GetOrderDetailsAsync(id);
        if (order == null)
        {
            return NotFound($"Order with ID {id} not found.");
        }

        await _orderService.CancelOrderAsync(id);
        return Ok(new { message = "Order has been canceled successfully." });
    }

    [HttpPost("{id}/update")]
    public async Task<IActionResult> UpdateOrderDetails(int id, [FromBody] OrderVM orderVm)
    {
        if (id != orderVm.OrderHeader.Id)
        {
            return BadRequest("Order ID mismatch.");
        }

        var order = await _orderService.GetOrderDetailsAsync(id);
        if (order == null)
        {
            return NotFound($"Order with ID {id} not found.");
        }

        await _orderService.UpdateOrderDetailsAsync(orderVm);
        return Ok(new { message = "Order has been updated successfully." });
    }
}