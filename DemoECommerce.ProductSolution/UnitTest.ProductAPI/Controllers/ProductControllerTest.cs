using eCommerce.SharedLibrary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;

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
            var response = new Response(true, "Product created successfully.");

            // Act //
            A.CallTo(() => _productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
            var result = await _productsController.CreateProduct(newProductDto);

            // Assert //
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var okResponse = okResult.Value as Response;
            okResponse.Should().NotBeNull();
            okResponse!.Flag.Should().BeTrue();
            okResponse!.Message.Should().Be("Product created successfully.");
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

        [Fact]
        public async Task CreateProduct_WhenCreationFails_ReturnBadRequestResponse()
        {
            // Arrange //
            var newProductDto = new ProductDTO(1, "New Product", 150.0m, 10);
            var response = new Response(false, "Product creation failed.");

            // Act //
            A.CallTo(() => _productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
            var result = await _productsController.CreateProduct(newProductDto);


            // Assert //
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = badRequestResult.Value as string;
            responseResult.Should().NotBeNull();
            responseResult.Should().Be("Product creation failed.");
        }


        // Update Product
        [Fact]
        public async Task UpdateProduct_WhenModelStateIsValid_ReturnOkResponse()
        {
            // Arrange //
            var updateProductDto = new ProductDTO(1, "Updated Product", 200.0m, 8);
            var response = new Response(true, "Product updated successfully.");

            // Act //
            A.CallTo(() => _productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);
            var result = await _productsController.UpdateProduct(updateProductDto);

            // Assert //
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var okResponse = okResult.Value as Response;
            okResponse.Should().NotBeNull();
            okResponse!.Flag.Should().BeTrue();
            okResponse!.Message.Should().Be("Product updated successfully.");
        }

        [Fact]
        public async Task UpdateProduct_WhenModelStateIsInvalid_ReturnBadRequestResponse()
        {
            // Arrange //
            var updateProductDto = new ProductDTO(1, "", 200.0m, 8); // Invalid Name
            // Manually add model state error
            _productsController.ModelState.AddModelError("Name", "The Name field is required.");

            // Act //
            var result = await _productsController.UpdateProduct(updateProductDto);

            // Assert //
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var modelState = badRequestResult.Value as SerializableError;
            modelState.Should().NotBeNull();
            modelState!.ContainsKey("Name").Should().BeTrue();
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateFails_ReturnBadRequestResponse()
        {
            // Arrange //
            var updateProductDto = new ProductDTO(1, "Updated Product", 200.0m, 8);
            var response = new Response(false, "Product update failed.");

            // Act //
            A.CallTo(() => _productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);
            var result = await _productsController.UpdateProduct(updateProductDto);

            // Assert //
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = badRequestResult.Value as string;
            responseResult.Should().NotBeNull();
            responseResult.Should().Be("Product update failed.");
        }

        // Delete Product
        [Fact]
        public async Task DeleteProduct_WhenCalled_ReturnOkResponse()
        {
            // Arrange //
            int productId = 1;
            var response = new Response(true, "Product deleted successfully.");
            var product = new ProductDTO(productId, "Product to Delete", 120.0m, 4);

            // Act //
            A.CallTo(() => _productInterface.DeleteAsync(productId)).Returns(response);
            var result = await _productsController.DeleteProduct(product);

            // Assert //
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var okResponse = okResult.Value as Response;
            okResponse.Should().NotBeNull();
            okResponse!.Flag.Should().BeTrue();
            okResponse!.Message.Should().Be("Product deleted successfully.");
        }

        [Fact]
        public async Task DeleteProduct_WhenDeletionFails_ReturnBadRequestResponse()
        {
            // Arrange //
            int productId = 1;
            var response = new Response(false, "Product deletion failed.");
            var product = new ProductDTO(productId, "Product to Delete", 120.0m, 4);

            // Act //
            A.CallTo(() => _productInterface.DeleteAsync(productId)).Returns(response);
            var result = await _productsController.DeleteProduct(product);

            // Assert //
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = badRequestResult.Value as string;
            responseResult.Should().NotBeNull();
            responseResult.Should().Be("Product deletion failed.");
        }
    }
}