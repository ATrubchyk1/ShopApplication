using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication_Utility;

namespace ShopApplication.Controllers;

[Authorize(Roles = Constants.AdminRole)]
public class ApplicationTypeController : Controller
{
    private readonly IApplicationTypeService _applicationTypeService;

    public ApplicationTypeController(IApplicationTypeService applicationTypeService)
    {
        _applicationTypeService = applicationTypeService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _applicationTypeService.GetAllAsync();
        return View(result);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ApplicationType applicationType)
    {
        await _applicationTypeService.AddAsync(applicationType);
        TempData[Constants.Success] = "ApplicationType created successfully";
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var obj = await _applicationTypeService.GetByIdAsync((int)id);
        if (obj == null)
        {
            return NotFound();
        }
        
        return View(obj);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(ApplicationType applicationType)
    {
        if (ModelState.IsValid)
        {
            _applicationTypeService.Update(applicationType);
            TempData[Constants.Success] = "ApplicationType update successfully";
            return RedirectToAction("Index");
        }
        TempData[Constants.Error] = "Error not updated";
        return View(applicationType);
    }
    
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var obj = await _applicationTypeService.GetByIdAsync(id.GetValueOrDefault());
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
        var result = await _applicationTypeService.GetByIdAsync(id.GetValueOrDefault());
        if (result == null)
        {
            return NotFound();
        }
        await _applicationTypeService.DeleteAsync(result);
        TempData[Constants.Success] = "ApplicationType deleted successfully";
        return RedirectToAction("Index");
    }
}