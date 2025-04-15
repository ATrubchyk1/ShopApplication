using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models.ViewModels;
using ShopApplication_Utility;

namespace ShopApplication.Controllers.ApiControllers;

[Authorize(Roles = Constants.AdminRole)]
[Route("api/[controller]")]
[ApiController]
public class ProductControllerApiController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductControllerApiController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productService.GetAllAsync(includes: "Category,ApplicationType");
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var productVm = await _productService.GetProductVmAsync(id);
        if (productVm.Product == null)
        {
            return NotFound($"Product with ID {id} not found.");
        }
        return Ok(productVm);
    }

    [HttpPost]
    public async Task<IActionResult> UpsertProduct([FromForm] ProductVM productVm)
    {
        if (productVm == null)
        {
            return BadRequest("Product data is required.");
        }

        var files = HttpContext.Request.Form.Files;
        var response = await _productService.UpsertProductAsync(productVm, files);
        if (response == 0)
        {
            return CreatedAtAction(nameof(GetProduct), new { id = productVm.Product.Id }, productVm);
        }
        if (response == 1)
        {
            return Ok(new { message = "Product updated successfully" });
        }
        return StatusCode(500, "An error occurred while processing the product.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productService.FirstOfDefaultAsync(x => x.Id == id, includes: "Category,ApplicationType");
        if (product == null)
        {
            return NotFound($"Product with ID {id} not found.");
        }

        var response = await _productService.DeleteProductAsync(id);
        if (response)
        {
            return Ok(new { message = "Product deleted successfully" });
        }
        else
        {
            return StatusCode(500, "An error occurred while deleting the product.");
        }
    }
}