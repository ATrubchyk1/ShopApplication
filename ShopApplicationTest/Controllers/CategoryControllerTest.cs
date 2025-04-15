using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication.Controllers;

namespace ShopApplicationTest.Controllers;

public class CategoryControllerTest
{
    private Mock<ICategoryService> _mockCategoryService;
        private CategoryController _controller;

        [SetUp]
        public void Setup()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _controller = new CategoryController(_mockCategoryService.Object);
            
            var tempDataProvider = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller.TempData = tempDataProvider;
        }
        
        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public async Task IndexAsync_ReturnsViewWithCategories()
        {
            var categories = new List<Category> { new Category { Id = 1, Name = "Test" } };
            _mockCategoryService.Setup(s => s.GetAllAsync(null, null, null, false)).ReturnsAsync(categories);

            var result = await _controller.Index();

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(categories));
        }

        [Test]
        public void Create_Get_ReturnsView()
        {
            var result = _controller.Create();
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            var category = new Category { Id = 1, Name = "Test" };

            var result = await _controller.Create(category);

            _mockCategoryService.Verify(s => s.AddAsync(category), Times.Once);
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Create_Post_InvalidModel_ReturnsView()
        {
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.Create(new Category());

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
        }

        [Test]
        public async Task Edit_Get_ValidId_ReturnsViewWithModel()
        {
            var category = new Category { Id = 1, Name = "Test" };
            _mockCategoryService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(category);

            var result = await _controller.Edit(1);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(category));
        }

        [Test]
        public async Task Edit_Get_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.Edit((int?)null);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Get_NonExistentId_ReturnsNotFound()
        {
            _mockCategoryService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Category)null);

            var result = await _controller.Edit(99);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void Edit_Post_ValidModel_RedirectsToIndex()
        {
            var category = new Category { Id = 1, Name = "Test" };

            var result = _controller.Edit(category);

            _mockCategoryService.Verify(s => s.Update(category), Times.Once);
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public void Edit_Post_InvalidModel_ReturnsView()
        {
            _controller.ModelState.AddModelError("Name", "Required");

            var result = _controller.Edit(new Category());

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Delete_Get_ValidId_ReturnsView()
        {
            var category = new Category { Id = 1, Name = "Test" };
            _mockCategoryService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(category);

            var result = await _controller.Delete(1);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(category));
        }

        [Test]
        public async Task Delete_Get_InvalidId_ReturnsNotFound()
        {
            var result = await _controller.Delete(null);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_Get_NonExistent_ReturnsNotFound()
        {
            _mockCategoryService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Category)null!);

            var result = await _controller.Delete(99);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeletePostAsync_ValidId_DeletesAndRedirects()
        {
            var category = new Category { Id = 1, Name = "Test" };
            _mockCategoryService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(category);

            var result = await _controller.DeletePost(1);

            _mockCategoryService.Verify(s => s.DeleteAsync(category), Times.Once);
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task DeletePostAsync_InvalidId_ReturnsNotFound()
        {
            _mockCategoryService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((Category)null!);

            var result = await _controller.DeletePost(999);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }
}