using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication_Models.ViewModels;
using ShopApplication.Controllers;

namespace ShopApplicationTest.Controllers;

public class ProductControllerTests
{
    private Mock<IProductService> _mockProductService;
        private ProductController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductController(_mockProductService.Object);
            
            var tempDataProvider = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller.TempData = tempDataProvider;
        }
        
        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public async Task IndexAsync_ReturnsViewWithProducts()
        {
            var products = new List<Product> { new Product { Id = 1, Name = "Test" } };
            _mockProductService.Setup(x => x.GetAllAsync(null, null, "Category,ApplicationType", false))
                .ReturnsAsync(products);

            var result = await _controller.Index();

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(products));
        }

        [Test]
        public async Task UpsertAsync_Get_ReturnsViewWithProductVM()
        {
            var productVm = new ProductVM { Product = new Product { Id = 1 } };
            _mockProductService.Setup(x => x.GetProductVmAsync(1)).ReturnsAsync(productVm);

            var result = await _controller.Upsert(1);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(productVm));
        }

        [Test]
        public async Task UpsertAsync_Get_ReturnsNotFoundForInvalidId()
        {
            var productVm = new ProductVM { Product = new Product { Id = 0 } };
            _mockProductService.Setup(x => x.GetProductVmAsync(1)).ReturnsAsync(productVm);

            var result = await _controller.Upsert(1);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task UpsertAsync_Post_RedirectsToIndex()
        {
            var productVm = new ProductVM { Product = new Product { Id = 1 } };
            var files = new FormFileCollection();
            _mockProductService.Setup(x => x.UpsertProductAsync(productVm, files)).ReturnsAsync(1);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(null, files);

            var result = await _controller.Upsert(productVm);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect?.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task DeleteAsync_ReturnsViewWithProduct()
        {
            var product = new Product { Id = 1 };
            _mockProductService.Setup(x => x.FirstOfDefaultAsync(It.IsAny<Expression<System.Func<Product, bool>>>(), "Category,ApplicationType", true))
                .ReturnsAsync(product);

            var result = await _controller.Delete(1);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(product));
        }

        [Test]
        public async Task DeleteAsync_ReturnsNotFound_WhenProductNull()
        {
            _mockProductService.Setup(x => x.FirstOfDefaultAsync(It.IsAny<Expression<System.Func<Product, bool>>>(), "Category,ApplicationType", true))
                .ReturnsAsync((Product)null!);

            var result = await _controller.Delete(1);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeletePostAsync_DeletesProductAndRedirects()
        {
            _mockProductService.Setup(x => x.DeleteProductAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeletePost(1);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            if (result is RedirectToActionResult redirect) Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }
}