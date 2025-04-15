using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication_Utility;

namespace ShopApplication.Controllers;

[Authorize(Roles = Constants.AdminRole)]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();
        return View(categories);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            await _categoryService.AddAsync(category);
            TempData[Constants.Success] = "Category created successfully";
            return RedirectToAction("Index");
        }
        TempData[Constants.Error] = "Error while creating category";
        return View(category);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        var obj = await _categoryService.GetByIdAsync(id.GetValueOrDefault());
        if (obj == null)
        {
            return NotFound();
        }
        return View(obj);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            _categoryService.Update(category);
            TempData[Constants.Success] = "Edit successfully";
            return RedirectToAction("Index");
        }
        TempData[Constants.Error] = "Error while edit";
        return View(category);
    }
    
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var obj = await _categoryService.GetByIdAsync(id.GetValueOrDefault());
        if (obj == null)
        {
            return NotFound();
        }
        
        return View(obj);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost(int? id)
    {
        var result = await _categoryService.GetByIdAsync(id.GetValueOrDefault());
        if (result == null)
        {
            TempData[Constants.Error] = "Error while delete"; 
            return NotFound();
        }
        await _categoryService.DeleteAsync(result);
        TempData[Constants.Success] = "Category deleted successfully";
        return RedirectToAction("Index");
    }
}