using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProduct productInterface) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
    {
        // Retrieve all products from the data source
        var products = await productInterface.GetAllAsync();

        if (products is null || !products.Any())
        {
            return NotFound("No products found!");
        }
        // Convert entities to DTOs
        var (_, list) = ProductConversions.FromEntity(null!, products);

        return list!.Any() ? Ok(list) : NotFound("No products found!");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> GetProductById(int id)
    {
        // Retrieve the product by ID from the data source
        var product = await productInterface.FindByIdAsync(id);
        if (product is null)
        {
            return NotFound($"Product requested not found!");
        }

        // Convert entity to DTO
        var (dto, _) = ProductConversions.FromEntity(product, null!);
        return dto is not null ? Ok(dto) : NotFound($"Product not found!");
    }

    [HttpPost]
    public async Task<ActionResult<Response>> CreateProduct(ProductDTO productDto)
    {
        // Validate the incoming DTO Data annotations
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Convert DTO to entity and create the product
        var getEntity = ProductConversions.ToEntity(productDto);
        var response = await productInterface.CreateAsync(getEntity);

        return response.Flag ? CreatedAtAction(nameof(GetProductById), new { id = getEntity.Id }) /*Ok(response)*/: BadRequest(response.Message);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateProduct(ProductDTO productDto)
    {
        // Validate the incoming DTO Data annotations
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Convert DTO to entity and update the product
        var getEntity = ProductConversions.ToEntity(productDto);
        var response = await productInterface.UpdateAsync(getEntity);

        return response.Flag ? Ok(response) : BadRequest(response.Message);
    }

    [HttpDelete]
    public async Task<ActionResult<Response>> DeleteProduct(ProductDTO productDto)
    {
        // Convert DTO to entity and delete the product
        var getEntity = ProductConversions.ToEntity(productDto);
        var response = await productInterface.DeleteAsync(getEntity.Id);

        return response.Flag ? Ok(response) : BadRequest(response.Message);
    }
}