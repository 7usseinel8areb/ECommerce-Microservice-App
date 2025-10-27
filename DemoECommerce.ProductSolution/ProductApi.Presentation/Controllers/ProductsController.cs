using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            var products = await productInterface.GetAllAsync();
            
            if (products is null || !products.Any())
            {
                return NotFound("No products found!");
            }

            var (_, list) = ProductConversions.FromEntity(null!, products);

            return list!.Any() ? Ok(list) : NotFound("No products found!");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            var product = await productInterface.FindByIdAsync(id);
            if (product is null)
            {
                return NotFound($"Product requested not found!");
            }
            var (dto, _) = ProductConversions.FromEntity(product, null!);
            return dto is not null ? Ok(dto) : NotFound($"Product not found!");
        }
    }
}
