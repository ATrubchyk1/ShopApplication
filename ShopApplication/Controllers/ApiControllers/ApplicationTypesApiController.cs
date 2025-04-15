using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models;
using ShopApplication_Utility;

namespace ShopApplication.Controllers.ApiControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Constants.AdminRole)]
public class ApplicationTypesApiController : ControllerBase
{
    private readonly IApplicationTypeService _applicationTypeService;

        public ApplicationTypesApiController(IApplicationTypeService applicationTypeService)
        {
            _applicationTypeService = applicationTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _applicationTypeService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _applicationTypeService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApplicationType? applicationType)
        {
            if (applicationType == null)
            {
                return BadRequest();
            }

            await _applicationTypeService.AddAsync(applicationType);
            return CreatedAtAction(nameof(GetById), new { id = applicationType.Id }, applicationType); // Возвращаем 201
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ApplicationType applicationType)
        {
            if (id != applicationType.Id)
            {
                return BadRequest();
            }

            var existingApplicationType = await _applicationTypeService.GetByIdAsync(id);
            if (existingApplicationType == null)
            {
                return NotFound();
            }

            _applicationTypeService.Update(applicationType);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var applicationType = await _applicationTypeService.GetByIdAsync(id);
            if (applicationType == null)
            {
                return NotFound();
            }
            await _applicationTypeService.DeleteAsync(applicationType);
            return NoContent();
        }
}