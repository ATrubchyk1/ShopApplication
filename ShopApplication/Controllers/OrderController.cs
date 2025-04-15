using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models.ViewModels;
using ShopApplication_Utility;

namespace ShopApplication.Controllers;

[Authorize(Roles = Constants.AdminRole)]
public class OrderController : Controller
{
    
    
    private readonly IOrderService _orderService;
    
    [BindProperty]
    public OrderVM OrderVM { get; set; }

    public OrderController(
        IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index(string searchName = null, string searchEmail = null, string searchPhone = null, string Status = null)
    {
        var orderListVm = new OrderListVM()
        {
            OrderHeaderList = await _orderService.GetAllAsync(),
            StatusList = Constants.StatusList.ToList().Select(x => new SelectListItem()
            {
                Text = x,
                Value = x
            })
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
        if (!string.IsNullOrEmpty(Status) && Status != "--Order Status--")
        {
            orderListVm.OrderHeaderList = orderListVm.OrderHeaderList.Where(x => x.OrderStatus.ToLower().Contains(Status.ToLower()));
        }
        return View(orderListVm);
    }

    public async Task<IActionResult> Details(int id)
    {
        OrderVM = await _orderService.GetOrderDetailsAsync(id);
        return View(OrderVM);
    }

    [HttpPost]
    public async Task<IActionResult> StartProcessing()
    {
        if (OrderVM.OrderHeader != null) await _orderService.StartProcessingOrderAsync(OrderVM.OrderHeader.Id);
        TempData[Constants.Success] = "Order is in Progress!";
        return RedirectToAction(nameof(System.Index));
    }
    
    [HttpPost]
    public async Task<IActionResult> ShipOrder()
    {
        if (OrderVM.OrderHeader != null) await _orderService.ShipOrderAsync(OrderVM.OrderHeader.Id);
        TempData[Constants.Success] = "Order has been Shipped successfully!";
        return RedirectToAction(nameof(System.Index));
    }
    
    [HttpPost]
    public async Task<IActionResult> CancelOrder()
    {
        if (OrderVM.OrderHeader != null) await _orderService.CancelOrderAsync(OrderVM.OrderHeader.Id);
        TempData[Constants.Success] = "Order has been Canceled successfully!";
        return RedirectToAction(nameof(System.Index));
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateOrderDetails()
    {
        await _orderService.UpdateOrderDetailsAsync(OrderVM);
        TempData[Constants.Success] = "Order has been Updated successfully!";
        if (OrderVM.OrderHeader != null)
            return RedirectToAction("Details", "Order", new { id = OrderVM.OrderHeader.Id });
        return NotFound();
    }
}