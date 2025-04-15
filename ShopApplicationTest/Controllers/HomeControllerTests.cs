using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication_Models.ViewModels;
using ShopApplication.Controllers;

namespace ShopApplicationTest.Controllers;

public class HomeControllerTests
{
    private HomeController _controller;
    private Mock<ILogger<HomeController>> _mockLogger;
    private Mock<IHomeService> _mockHomeService;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<HomeController>>();
        _mockHomeService = new Mock<IHomeService>();
        _controller = new HomeController(_mockLogger.Object, _mockHomeService.Object);
        
        var tempDataProvider = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        _controller.TempData = tempDataProvider;
    }
    
    [TearDown]
    public void TearDown()
    {
        _controller.Dispose();
    }

    [Test]
    public async Task IndexAsync_ReturnsViewWithHomeViewModel()
    {
        var viewModel = new HomeVM();
        _mockHomeService.Setup(s => s.GetHomeViewModelAsync()).ReturnsAsync(viewModel);

        var result = await _controller.Index();

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.That(viewResult.Model, Is.EqualTo(viewModel));
    }

    [Test]
    public async Task DetailsAsync_ReturnsViewWithDetailsViewModel()
    {
        var viewModel = new DetailsVM();
        _mockHomeService.Setup(s => s.GetDetailsViewModelAsync(1)).ReturnsAsync(viewModel);

        var result = await _controller.Details(1);

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.That(viewResult.Model, Is.EqualTo(viewModel));
    }

    [Test]
    public void DetailsPost_ValidProduct_AddsToCartAndRedirects()
    {
        var detailsVm = new DetailsVM
        {
            Product = new Product { TempUnits = 2 }
        };

        var result = _controller.Details(1, detailsVm);

        _mockHomeService.Verify(s => s.AddToCart(1, 2), Times.Once);
        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirect = result as RedirectToActionResult;
        Assert.That(redirect?.ActionName, Is.EqualTo("Index"));
    }

    [Test]
    public void RemoveFromCart_ValidId_RemovesFromCartAndRedirects()
    {
        var result = _controller.RemoveFromCart(1);

        _mockHomeService.Verify(s => s.RemoveFromCart(1), Times.Once);
        Assert.IsInstanceOf<RedirectToActionResult>(result);
        var redirect = result as RedirectToActionResult;
        Assert.That(redirect?.ActionName, Is.EqualTo("Index"));
    }
}