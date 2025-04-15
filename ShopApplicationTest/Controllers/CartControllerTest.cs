using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication_Models.ViewModels;
using ShopApplication_Utility.BrainTree;
using ShopApplication.Controllers;

namespace ShopApplicationTest.Controllers;

public class CartControllerTest
{
   public class CartControllerTests
    {
        private Mock<ICartService> _mockCartService;
        private Mock<IEmailSenderService> _mockEmailSenderService;
        private Mock<IBrainTreeGate> _mockBrainTreeService;
        private CartController _controller;

        [SetUp]
        public void Setup()
        {
            _mockCartService = new Mock<ICartService>();
            _mockEmailSenderService = new Mock<IEmailSenderService>();
            _mockBrainTreeService = new Mock<IBrainTreeGate>();
            _controller = new CartController(
                _mockCartService.Object, 
                _mockEmailSenderService.Object, 
                _mockBrainTreeService.Object);

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user123")
            };
            var userIdentity = new ClaimsIdentity(userClaims);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal(userIdentity) }
            };
            
            var tempDataProvider = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller.TempData = tempDataProvider;
        }
        
        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public async Task IndexAsync_ReturnsViewWithCartProducts()
        {
            var products = new List<Product> { new Product { Id = 1, Name = "Product 1" } };
            _mockCartService.Setup(s => s.GetCartProductsAsync()).ReturnsAsync(products);

            var result = await _controller.Index();

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(products));
        }

        [Test]
        public Task IndexPost_UpdatesCartAndRedirectsToSummary()
        {
            var products = new List<Product> { new Product { Id = 1, Name = "Product 1" } };
            _mockCartService.Setup(s => s.UpdateCart(It.IsAny<IEnumerable<Product>>()));

            var result = _controller.IndexPost(products);

            _mockCartService.Verify(s => s.UpdateCart(products), Times.Once);
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("SummaryAsync"));
            return Task.CompletedTask;
        }

        [Test]
        public async Task SummaryAsync_ReturnsSummaryView()
        {
            var productUserVm = new ProductUserVM();
            _mockCartService.Setup(s => s.GetSummaryAsync(It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(productUserVm);
            _mockBrainTreeService.Setup(b => b.GetClientBrainTreeToken()).Returns("mockToken");

            var result = await _controller.Summary();

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(productUserVm));
        }

        [Test]
        public async Task SummaryPost_ProcessPaymentAndRedirects()
        {
            var productUserVm = new ProductUserVM();
            var collection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "payment_method_nonce", "nonce_value" }
            });
            _mockCartService.Setup(s => s.CreateOrderAsync(It.IsAny<ProductUserVM>(), It.IsAny<string>())).ReturnsAsync(1);
            _mockCartService.Setup(s => s.ProcessPaymentAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);
            _mockEmailSenderService.Setup(s => s.SendEmailAsync(It.IsAny<int>(), It.IsAny<ProductUserVM>()));

            var result = await _controller.SummaryPost(collection, productUserVm);

            _mockCartService.Verify(s => s.ProcessPaymentAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            _mockEmailSenderService.Verify(s => s.SendEmailAsync(It.IsAny<int>(), It.IsAny<ProductUserVM>()), Times.Once);
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("InquiryConfirmation"));
        }

        [Test]
        public async Task InquiryConfirmationAsync_ReturnsViewWithOrderDetails()
        {
            var orderHeader = new OrderHeader();
            _mockCartService.Setup(s => s.GetOrderByIdAsync(It.IsAny<int>())).ReturnsAsync(orderHeader);
            _mockCartService.Setup(s => s.ClearCart());

            var result = await _controller.InquiryConfirmation(1);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(orderHeader));
        }

        [Test]
        public void Remove_RemovesProductAndRedirectsToIndex()
        {
            _mockCartService.Setup(s => s.RemoveFromCart(It.IsAny<int>()));

            var result = _controller.Remove(1);

            _mockCartService.Verify(s => s.RemoveFromCart(1), Times.Once);
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public void Clear_ClearsCartAndRedirectsToIndex()
        {
            _mockCartService.Setup(s => s.ClearCart());

            var result = _controller.Clear();

            _mockCartService.Verify(s => s.ClearCart(), Times.Once);
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
        }
    }
} 