using eCommerce.SharedLibrary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ProductAPI.Controllers
{
    public class ProductControllerTest
    {
        private readonly IProduct _productInterface;
        private readonly ProductsController _productsController;


        public ProductControllerTest()
        {
            // Set up dependencies
            _productInterface = A.Fake<IProduct>();

            // Set up system under test (SUT)
            _productsController = new ProductsController(_productInterface);
        }

        // Get All Products
        [Fact]
        public async Task GetProduct_WhenProductExists_ReturnOkResponseWithProducts()
        {
            // Arrange //
            var products = new List<Product>()
            {
                new Product { Id = 1, Name = "Product 1", Price = 100.0m, Quantity = 5 },
                new Product { Id = 2, Name = "Product 2", Price = 208.0m, Quantity = 3 }
            };

            // set up fake response for GetAllAsync Method
            A.CallTo(() => _productInterface.GetAllAsync()).Returns(products);

            // Act //
            var result = await _productsController.GetAllProducts();


            // Assert //
            var okResult = result.Result as OkObjectResult;

            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedProducts = okResult.Value as IEnumerable<ProductDTO>;
            returnedProducts.Should().NotBeNull();
            returnedProducts!.Should().HaveCount(products.Count);
            returnedProducts!.First().Id.Should().Be(1);
            returnedProducts!.Last().Id.Should().Be(2);
        }

        [Fact]
        public async Task GetProduct_WhenNoProductsExist_ReturnNotFoundResponse()
        {
            // Arrange //
            var products = new List<Product>(); // Empty list
            // set up fake response for GetAllAsync Method
            A.CallTo(() => _productInterface.GetAllAsync()).Returns(products);

            // Act //
            var result = await _productsController.GetAllProducts();

            // Assert //
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            var message = notFoundResult.Value as string;
            message.Should().Be("No products found!");
        }



        // Create Product
        [Fact]
        public async Task CreateProduct_WhenModelStateIsValid_ReturnOkResponse()
        {
            // Arrange //
            var newProductDto = new ProductDTO(1, "New Product", 150.0m, 10);

            // set up fake response for CreateAsync Method
            A.CallTo(() => _productInterface.CreateAsync(A<Product>.Ignored))
                .Returns(new Response(true, "Product created successfully."));

            // Act //
            var result = await _productsController.CreateProduct(newProductDto);
            
            // Assert //
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
            var message = okResult.Value as string;
            message.Should().Be("Product created successfully.");
        }


        [Fact]
        public async Task CreateProduct_WhenModelStateIsInvalid_ReturnBadRequestResponse()
        {
            // Arrange //
            var newProductDto = new ProductDTO(1, "", 150.0m, 10); // Invalid Name

            // Manually add model state error
            _productsController.ModelState.AddModelError("Name", "The Name field is required.");

            // Act //
            var result = await _productsController.CreateProduct(newProductDto);

            // Assert //
            var badRequestResult = result.Result as BadRequestObjectResult;

            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var modelState = badRequestResult.Value as SerializableError;
            modelState.Should().NotBeNull();
            modelState!.ContainsKey("Name").Should().BeTrue();
        }
    }
}
