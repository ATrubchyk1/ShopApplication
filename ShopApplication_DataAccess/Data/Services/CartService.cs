using Braintree;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication_Models.ViewModels;
using ShopApplication_Utility;
using ShopApplication_Utility.BrainTree;

namespace ShopApplication_DataAccess.Data.Services;

public class CartService : ICartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProductRepository _productRepo;
    private readonly IOrderHeaderRepository _orderHeaderRepo;
    private readonly IOrderDetailRepository _orderDetailRepo;
    private readonly IApplicationUserRepository _userRepo;
    private readonly IBrainTreeGate _brainTree;

    public CartService(IHttpContextAccessor httpContextAccessor,
        IProductRepository productRepo,
        IOrderHeaderRepository orderHeaderRepo,
        IOrderDetailRepository orderDetailRepo,
        IBrainTreeGate brainTree, 
        IApplicationUserRepository userRepo)
    {
        _httpContextAccessor = httpContextAccessor;
        _productRepo = productRepo;
        _orderHeaderRepo = orderHeaderRepo;
        _orderDetailRepo = orderDetailRepo;
        _brainTree = brainTree;
        _userRepo = userRepo;
    }

    public async Task<List<Product>> GetCartProductsAsync()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        var shoppingCartList = session.Get<List<ShoppingCart>>(Constants.SessionCart) ?? new List<ShoppingCart>();

        var productsInCart = shoppingCartList.Select(x => x.ProductId).ToList();
        var products = await _productRepo.GetAllAsync(p => productsInCart.Contains(p.Id));

        foreach (var item in shoppingCartList)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null)
                product.TempUnits = item.Units;
        }

        return products.ToList();
    }

    public void UpdateCart(IEnumerable<Product> products)
    {
        var shoppingCartList = products.Select(p => new ShoppingCart
        {
            ProductId = p.Id,
            Units = p.TempUnits
        }).ToList();

        _httpContextAccessor.HttpContext?.Session.Set(Constants.SessionCart, shoppingCartList);
    }

    public void RemoveFromCart(int id)
    {
        var shoppingCartList = _httpContextAccessor.HttpContext?.Session.Get<List<ShoppingCart>>(Constants.SessionCart) ?? new List<ShoppingCart>();
        var itemToRemove = shoppingCartList.FirstOrDefault(x => x.ProductId == id);
        if (itemToRemove != null)
        {
            shoppingCartList.Remove(itemToRemove);
            _httpContextAccessor.HttpContext?.Session.Set(Constants.SessionCart, shoppingCartList);
        }
    }

    public void ClearCart()
    {
        _httpContextAccessor.HttpContext?.Session.Clear();
    }

    public async Task<ProductUserVM> GetSummaryAsync(string userId, bool isAdmin)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        ApplicationUser? applicationUser;

        if (isAdmin)
        {
            var inquiryId = httpContext.Session.Get<int>(Constants.SessionInquiryId);
            if (inquiryId != 0)
            {
                var inquiryHeader = await _orderHeaderRepo.FirstOrDefaultAsync(x => x.Id == inquiryId);
                applicationUser = new ApplicationUser
                {
                    Email = inquiryHeader.Email,
                    PhoneNumber = inquiryHeader.PhoneNumber,
                    FullName = inquiryHeader.FullName
                };
            }
            else
            {
                applicationUser = new ApplicationUser();
            }
        }
        else
        {
            applicationUser = await _userRepo.FirstOrDefaultAsync(x => x.Id == userId);
        }

        var shoppingCartList = httpContext.Session.Get<List<ShoppingCart>>(Constants.SessionCart) ?? new List<ShoppingCart>();

        var productIds = shoppingCartList.Select(x => x.ProductId).ToList();
        var productList = await _productRepo.GetAllAsync(x => productIds.Contains(x.Id));

        var productUserVm = new ProductUserVM
        {
            ApplicationUser = applicationUser
        };

        foreach (var cart in shoppingCartList)
        {
            var productTemp = productList.FirstOrDefault(x => x.Id == cart.ProductId);
            if (productTemp != null)
            {
                productTemp.TempUnits = cart.Units;
                productUserVm.ProductsList.Add(productTemp);
            }
        }
        return productUserVm;
    }

    public async Task<int> CreateOrderAsync(ProductUserVM productUserVm, string userId)
    {
        if (productUserVm.ApplicationUser != null)
        {
            var orderHeader = new OrderHeader
            {
                CreatedByUserId = userId,
                FinalOrderTotal = productUserVm.ProductsList.Sum(p => p.Price * p.TempUnits),
                City = productUserVm.ApplicationUser.City,
                StreetAddress = productUserVm.ApplicationUser.StreetAddress,
                State = productUserVm.ApplicationUser.State,
                PostalCode = productUserVm.ApplicationUser.PostCode,
                FullName = productUserVm.ApplicationUser.FullName,
                PhoneNumber = productUserVm.ApplicationUser.PhoneNumber,
                Email = productUserVm.ApplicationUser.Email,
                OrderDate = DateTime.Now,
                OrderStatus = Constants.StatusPending
            };

            await _orderHeaderRepo.AddAsync(orderHeader);
            await _orderHeaderRepo.SaveAsync();

            foreach (var product in productUserVm.ProductsList)
            {
                await _orderDetailRepo.AddAsync(new OrderDetail
                {
                    OrderHeaderId = orderHeader.Id,
                    PricePerSqFt = product.Price,
                    Sqft = product.TempUnits,
                    ProductId = product.Id
                });
            }

            await _orderDetailRepo.SaveAsync();
            return orderHeader.Id;
        }
        return 0;
    }

    public async Task<bool> ProcessPaymentAsync(int orderId, string nonce)
    {
        var order = await _orderHeaderRepo.FirstOrDefaultAsync(o => o.Id == orderId, isTracking: true);
        
        var request = new TransactionRequest
        {
            Amount = Convert.ToDecimal(order.FinalOrderTotal),
            PaymentMethodNonce = nonce,
            OrderId = order.Id.ToString(),
            Options = new TransactionOptionsRequest { SubmitForSettlement = true }
        };

        var gateway = _brainTree.GetGateWay();
        var result = await gateway.Transaction.SaleAsync(request);

        if (result.IsSuccess())
        {
            order.TransactionId = result.Target.Id;
            order.OrderStatus = Constants.StatusApproved;
        }
        else
        {
            order.OrderStatus = Constants.StatusCancelled;
        }
        await _orderHeaderRepo.SaveAsync();
        return result.IsSuccess();
    }

    public async Task<OrderHeader?> GetOrderByIdAsync(int? id)
    {
        return await _orderHeaderRepo.FirstOrDefaultAsync(o => o.Id == id);
    }
}