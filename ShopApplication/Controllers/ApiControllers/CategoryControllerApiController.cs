using Microsoft.AspNetCore.Mvc;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;

namespace ShopApplication.Controllers.ApiControllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryControllerApiController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryControllerApiController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        var categories = await _categoryService.GetAllAsync();
        if (categories == null || !categories.Any())
        {
            return NotFound("No categories found.");
        }
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound($"Category with ID {id} not found.");
        }
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
    {
        if (category == null || !ModelState.IsValid)
        {
            return BadRequest("Invalid category data.");
        }

        await _categoryService.AddAsync(category);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
    {
        if (id != category.Id || !ModelState.IsValid)
        {
            return BadRequest("Category ID mismatch or invalid data.");
        }

        var existingCategory = await _categoryService.GetByIdAsync(id);
        if (existingCategory == null)
        {
            return NotFound($"Category with ID {id} not found.");
        }

        _categoryService.Update(category);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound($"Category with ID {id} not found.");
        }

        await _categoryService.DeleteAsync(category);
        return NoContent();
    }
}