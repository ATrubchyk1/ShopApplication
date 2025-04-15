using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication.Controllers;

namespace ShopApplicationTest.Controllers;

public class ApplicationTypeControllerTest
{
    private Mock<IApplicationTypeService> _mockService;
    private ApplicationTypeController _controller;

    [SetUp]
    public void Setup()
    {
    _mockService = new Mock<IApplicationTypeService>();

    _controller = new ApplicationTypeController(_mockService.Object);
    
    var tempDataProvider = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
    _controller.TempData = tempDataProvider;
    }

    [TearDown]
    public void TearDown()
    {
        _controller.Dispose();
    }

    [Test]
    public async Task IndexAsync_ShouldReturnsViewWithData()
    {
        var data = new List<ApplicationType> { new ApplicationType { Id = 1, Name = "Test" } };
        _mockService.Setup(s => s.GetAllAsync(
            It.IsAny<Expression<Func<ApplicationType, bool>>?>(),
            It.IsAny<Func<IQueryable<ApplicationType>, IOrderedQueryable<ApplicationType>>?>(),
            It.IsAny<string?>(),
            It.IsAny<bool>()
        )).ReturnsAsync(data);
        
        var result = await _controller.Index();

        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.That(viewResult.Model, Is.EqualTo(data));
    }
    
    [Test]
    public void Create_Get_ReturnsView()
    {
        var result = _controller.Create();
        Assert.IsInstanceOf<ViewResult>(result);
    }
    
    [Test]
    public async Task Create_Post_RedirectsOnSuccess()
    {
        var appType = new ApplicationType { Id = 1, Name = "Test" };
        _mockService.Setup(s => s.AddAsync(It.IsAny<ApplicationType>())).Returns(Task.CompletedTask);
        
        var result = await _controller.Create(appType);

        _mockService.Verify(s => s.AddAsync(appType), Times.Once);

        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult); // Проверка на RedirectToActionResult
        Assert.That(redirectResult?.ActionName, Is.EqualTo("Index")); // Проверка редиректа на "Index"
    }
    
    [Test]
        public async Task EditAsync_Get_ReturnsView_WhenIdIsValid()
        {
            var appType = new ApplicationType { Id = 1 };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(appType);

            var result = await _controller.Edit(1);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(appType));
        }

        [Test]
        public async Task EditAsync_Get_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await _controller.Edit((int?)null);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void Edit_Post_Redirects_WhenModelIsValid()
        {
            var appType = new ApplicationType { Id = 1, Name = "Valid" };
            _controller.ModelState.Clear();

            var result = _controller.Edit(appType);

            _mockService.Verify(s => s.Update(appType), Times.Once);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public void Edit_Post_ReturnsView_WhenModelIsInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");

            var appType = new ApplicationType();

            var result = _controller.Edit(appType);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task DeleteAsync_ReturnsView_WhenValidId()
        {
            var appType = new ApplicationType { Id = 1 };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(appType);

            var result = await _controller.Delete(1);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
        }

        [Test]
        public async Task DeletePostAsync_Redirects_WhenSuccess()
        {
            var appType = new ApplicationType { Id = 1 };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(appType);

            var result = await _controller.DeletePost(1);

            _mockService.Verify(s => s.DeleteAsync(appType), Times.Once);
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
}