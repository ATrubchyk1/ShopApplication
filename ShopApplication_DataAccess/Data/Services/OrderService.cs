using Braintree;
using Microsoft.AspNetCore.Identity.UI.Services;
using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication_Models.ViewModels;
using ShopApplication_Utility;
using ShopApplication_Utility.BrainTree;

namespace ShopApplication_DataAccess.Data.Services;

public class OrderService : IOrderService
{
    private readonly IOrderHeaderRepository _orderHeaderRepository;
    private readonly IOrderDetailRepository _orderDetailRepository;
    private readonly IBrainTreeGate _brainService;

    public OrderService(
        IOrderHeaderRepository orderHeaderRepository,
        IOrderDetailRepository orderDetailRepository,
        IBrainTreeGate brainService)
    {
        _orderHeaderRepository = orderHeaderRepository;
        _orderDetailRepository = orderDetailRepository;
        _brainService = brainService;
    }


    public async Task<OrderVM> GetOrderDetailsAsync(int id)
    {
        var orderHeader = await _orderHeaderRepository.FirstOrDefaultAsync(x => x.Id == id);
        var orderDetail = await _orderDetailRepository.GetAllAsync(x => x.OrderHeaderId == id, includes: "Product");

        var orderVm = new OrderVM
        {
            OrderHeader = orderHeader,
            OrderDetail = orderDetail
        };

        return orderVm;
    }

    public async Task StartProcessingOrderAsync(int orderId)
    {
        var orderHeader = await _orderHeaderRepository.FirstOrDefaultAsync(x => x.Id == orderId);
        if (orderHeader != null)
        {
            orderHeader.OrderStatus = Constants.StatusInProcess;

            _orderHeaderRepository.Update(orderHeader);
        }

        await _orderHeaderRepository.SaveAsync();
    }

    public async Task ShipOrderAsync(int orderId)
    {
        var orderHeader = await _orderHeaderRepository.FirstOrDefaultAsync(x => x.Id == orderId);
        if (orderHeader != null)
        {
            orderHeader.OrderStatus = Constants.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            _orderHeaderRepository.Update(orderHeader);
        }

        await _orderHeaderRepository.SaveAsync();
    }

    public async Task CancelOrderAsync(int orderId)
    {
        var orderHeader = await _orderHeaderRepository.FirstOrDefaultAsync(x => x.Id == orderId);
        
        // Open the payment gateway
        var gateway = _brainService.GetGateWay();
        if (orderHeader != null)
        {
            var transaction = gateway.Transaction.Find(orderHeader.TransactionId);

            if (transaction.Status == TransactionStatus.AUTHORIZED || transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                // Void the transaction (no refund)
                gateway.Transaction.Void(orderHeader.TransactionId);
            }
            else
            {
                // Refund the transaction
                gateway.Transaction.Refund(orderHeader.TransactionId);
            }
        }

        if (orderHeader != null)
        {
            orderHeader.OrderStatus = Constants.StatusRefunded;

            _orderHeaderRepository.Update(orderHeader);
        }

        await _orderHeaderRepository.SaveAsync();
    }

    public async Task UpdateOrderDetailsAsync(OrderVM orderVm)
    {
        var orderHeaderFromDb = await _orderHeaderRepository.FirstOrDefaultAsync(x => orderVm.OrderHeader != null && x.Id == orderVm.OrderHeader.Id);

        if (orderHeaderFromDb != null)
        {
            orderHeaderFromDb.FullName = orderVm.OrderHeader.FullName;
            orderHeaderFromDb.Email = orderVm.OrderHeader.Email;
            orderHeaderFromDb.PhoneNumber = orderVm.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = orderVm.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = orderVm.OrderHeader.City;
            orderHeaderFromDb.State = orderVm.OrderHeader.State;
            orderHeaderFromDb.PostalCode = orderVm.OrderHeader.PostalCode;

            _orderHeaderRepository.Update(orderHeaderFromDb);
        }

        await _orderHeaderRepository.SaveAsync();
    }

    public async Task<IEnumerable<OrderHeader>> GetAllAsync()
    {
        return await _orderHeaderRepository.GetAllAsync();
    }
}