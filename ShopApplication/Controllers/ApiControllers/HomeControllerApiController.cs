using Microsoft.AspNetCore.Mvc;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_Models.ViewModels;
using Swashbuckle.AspNetCore.Annotations;

namespace ShopApplication.Controllers.ApiControllers;

[ApiController]
[Route("api/[controller]")]
public class HomeControllerApiController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;
        private readonly IHomeService _homeService;

        public HomeControllerApiController(ILogger<HomeController> logger, IHomeService homeService)
        {
            _logger = logger;
            _homeService = homeService;
        }

        [HttpGet]
        public async Task<ActionResult<HomeVM>> GetHomeViewModel()
        {
            _logger.LogInformation("Index action called at {Time}", DateTime.UtcNow);
            var viewModel = await _homeService.GetHomeViewModelAsync();
            if (viewModel == null)
            {
                return NotFound("Home view model not found.");
            }
            return Ok(viewModel);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DetailsVM>> GetDetails(int id)
        {
            _logger.LogInformation("Details action called with id {ProductId} at {Time}", id, DateTime.UtcNow);
            var detailsVm = await _homeService.GetDetailsViewModelAsync(id);
            if (detailsVm == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            return Ok(detailsVm);
        }

        [HttpPost("details/{id}")]
        public IActionResult AddToCart(int id, [FromBody] DetailsVM detailsVm)
        {
            if (detailsVm?.Product != null)
            {
                _logger.LogInformation("DetailsPost action called with id {ProductId} and quantity {Quantity} at {Time}",
                    id, detailsVm.Product.TempUnits, DateTime.UtcNow);

                try
                {
                    _homeService.AddToCart(id, detailsVm.Product.TempUnits);
                    _logger.LogInformation("Product with id {ProductId} successfully added to cart at {Time}", id,
                        DateTime.UtcNow);
                    return Ok("Product added to cart!");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while adding product with id {ProductId} to cart at {Time}", id,
                        DateTime.UtcNow);
                    return StatusCode(500, "Failed to add product to cart.");
                }
            }

            return BadRequest("Invalid product data.");
        }

        [HttpDelete("cart/{id}")]
        public IActionResult RemoveFromCart(int id)
        {
            _logger.LogInformation("RemoveFromCart action called with id {ProductId} at {Time}", id, DateTime.UtcNow);
            try
            {
                _homeService.RemoveFromCart(id);
                _logger.LogInformation("Product with id {ProductId} removed from cart at {Time}", id, DateTime.UtcNow);
                return Ok("Product removed from cart.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing product with id {ProductId} from cart at {Time}", id, DateTime.UtcNow);
                return StatusCode(500, "Error occurred while removing product from cart.");
            }
        }
}