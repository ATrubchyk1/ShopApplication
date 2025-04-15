using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models.ViewModels;
using ShopApplication_Utility;

namespace ShopApplication.Controllers;

[Authorize(Roles = Constants.AdminRole)]
public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllAsync(includes: "Category,ApplicationType");
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Upsert(int? id)
    {
        var productVm = await _productService.GetProductVmAsync(id.GetValueOrDefault());
        if (id != null && productVm.Product.Id == 0)
        {
            return NotFound();
        }
        return View(productVm);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(ProductVM productVm)
    {
        var files = HttpContext.Request.Form.Files;
        var response = await _productService.UpsertProductAsync(productVm, files);
        TempData[Constants.Success] = response switch
        {
            0 => "Product created successfully",
            1 => "Product updated successfully",
            _ => TempData[Constants.Success]
        };
        return RedirectToAction("Index");
    }

    
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        var product = await _productService.FirstOfDefaultAsync(x => x.Id == id,includes: "Category,ApplicationType");
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost(int? id)
    {
        var response =  await _productService.DeleteProductAsync(id.GetValueOrDefault());
        if (response)
        {
            TempData[Constants.Success] = "Product deleted successfully";
        }
        return RedirectToAction("Index");
    }
}