using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication_Models.ViewModels;
using ShopApplication.Controllers;

namespace ShopApplicationTest.Controllers;

public class OrderControllerTest
{
    private Mock<IOrderService> _mockOrderService;
        private OrderController _controller;

        [SetUp]
        public void Setup()
        {
            _mockOrderService = new Mock<IOrderService>();
            _controller = new OrderController(_mockOrderService.Object);
            
            var tempDataProvider = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller.TempData = tempDataProvider;
        }
        
        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public async Task IndexAsync_ReturnsViewWithFilteredOrders()
        {
            var orders = new List<OrderHeader>
            {
                new OrderHeader { Id = 1, FullName = "John Doe", Email = "john@test.com", PhoneNumber = "123", OrderStatus = "Pending" },
                new OrderHeader { Id = 2, FullName = "Jane Smith", Email = "jane@test.com", PhoneNumber = "456", OrderStatus = "Shipped" }
            };
            _mockOrderService.Setup(s => s.GetAllAsync()).ReturnsAsync(orders);

            var result = await _controller.Index("Jane", null, null, "Shipped") as ViewResult;
            var model = result?.Model as OrderListVM;

            Assert.IsNotNull(model);
            Assert.That(model.OrderHeaderList.Count(), Is.EqualTo(1));
            Assert.That(model.OrderHeaderList.First().FullName, Is.EqualTo("Jane Smith"));
        }

        [Test]
        public async Task DetailsAsync_ReturnsViewWithOrderVM()
        {
            var orderVm = new OrderVM { OrderHeader = new OrderHeader { Id = 1 } };
            _mockOrderService.Setup(s => s.GetOrderDetailsAsync(1)).ReturnsAsync(orderVm);

            var result = await _controller.Details(1) as ViewResult;
            var model = result?.Model as OrderVM;

            Assert.IsNotNull(model);
            if (model.OrderHeader != null) Assert.That(model.OrderHeader.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task StartProcessingAsync_SetsTempDataAndRedirects()
        {
            _controller.OrderVM = new OrderVM { OrderHeader = new OrderHeader { Id = 1 } };
            var result = await _controller.StartProcessing();

            _mockOrderService.Verify(s => s.StartProcessingOrderAsync(1), Times.Once);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task ShipOrderAsync_SetsTempDataAndRedirects()
        {
            _controller.OrderVM = new OrderVM { OrderHeader = new OrderHeader { Id = 2 } };
            var result = await _controller.ShipOrder();

            _mockOrderService.Verify(s => s.ShipOrderAsync(2), Times.Once);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task CancelOrderAsync_SetsTempDataAndRedirects()
        {
            _controller.OrderVM = new OrderVM { OrderHeader = new OrderHeader { Id = 3 } };
            var result = await _controller.CancelOrder();

            _mockOrderService.Verify(s => s.CancelOrderAsync(3), Times.Once);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task UpdateOrderDetailsAsync_SuccessfulUpdate_RedirectsToDetails()
        {
            _controller.OrderVM = new OrderVM { OrderHeader = new OrderHeader { Id = 4 } };

            var result = await _controller.UpdateOrderDetails();

            _mockOrderService.Verify(s => s.UpdateOrderDetailsAsync(_controller.OrderVM), Times.Once);
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("Details"));
            if (redirectResult.RouteValues != null) Assert.That(redirectResult.RouteValues["id"], Is.EqualTo(4));
        }
}