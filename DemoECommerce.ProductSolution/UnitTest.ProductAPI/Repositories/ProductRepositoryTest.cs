using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace UnitTest.ProductAPI.Repositories
{
    public class ProductRepositoryTest
    {
        private readonly ProductDbContext _context;
        private readonly ProductRepository _repository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase("ProductDb")
                .Options;

            _context = new ProductDbContext(options);
            _repository = new ProductRepository(_context);
        }

        // Create Product Test
        [Fact]
        public async Task CreateAsync_WhenProductAlreadyExists_ReturnsFailureResponse()
        {
            // Arrange //
            var existingProduct = new Product { Name = "Test Product" };

            _context.Products.Add(existingProduct);
            await _context.SaveChangesAsync();

            // Act //
            var response = await _repository.CreateAsync(existingProduct);

            // Assert //
            response.Should().NotBeNull();
            response.Flag.Should().BeFalse();
            response.Message.Should().Be("Test Product already added.");

            Assert.False(response.Flag);
            Assert.Equal("Test Product already added.", response.Message);
        }

        [Fact]
        public async Task CreateAsync_WhenNewProductIsAdded_ReturnsSuccessResponse()
        {
            // Arrange //
            var newProduct = new Product { Name = "New Product" };

            // Act //
            var response = await _repository.CreateAsync(newProduct);

            // Assert //
            response.Should().NotBeNull();
            response.Flag.Should().BeTrue();
            response.Message.Should().Be("Product created successfully.");

            Assert.True(response.Flag);
            Assert.Equal("Product created successfully.", response.Message);
        }

        // Delete Product 
        [Fact]
        public async Task DeleteAsync_WhenProductExists_ReturnsSuccessResponse()
        {
            // Arrange //
            var product = new Product { Name = "Product to Delete" };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act //
            var response = await _repository.DeleteAsync(product.Id);

            // Assert //
            response.Should().NotBeNull();
            response.Flag.Should().BeTrue();
            response.Message.Should().Be("Product deleted successfully.");

            Assert.True(response.Flag);
            Assert.Equal("Product deleted successfully.", response.Message);
        }

        [Fact]
        public async Task DeleteAsync_WhenProductDoesNotExist_ReturnsFailureResponse()
        {
            // Arrange //
            var nonExistentProductId = 999;

            // Act //
            var response = await _repository.DeleteAsync(nonExistentProductId);

            // Assert //
            response.Should().NotBeNull();
            response.Flag.Should().BeFalse();
            response.Message.Should().Be("Product not found.");

            Assert.False(response.Flag);
            Assert.Equal("Product not found.", response.Message);
        }

        // FindById Test
        [Fact]
        public async Task FindByIdAsync_WhenProductExists_ReturnsProduct()
        {
            // Arrange //
            var product = new Product { Name = "Existing Product" };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act //
            var result = await _repository.FindByIdAsync(product.Id);

            // Assert //
            result.Should().NotBeNull();
            result!.Id.Should().Be(product.Id);
            result.Name.Should().Be("Existing Product");
            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal("Existing Product", result.Name);
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductDoesNotExist_ReturnsNull()
        {
            // Arrange //
            var nonExistentProductId = 999;

            // Act //
            var result = await _repository.FindByIdAsync(nonExistentProductId);

            // Assert //
            result.Should().BeNull();
            Assert.Null(result);
        }

        // GetAll 
        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnsAllProducts()
        {
            // Arrange //
            _context.Products.AddRange(
                new Product { Name = "Product 1" },
                new Product { Name = "Product 2" }
            );
            await _context.SaveChangesAsync();

            // Act //
            var result = await _repository.GetAllAsync();

            // Assert //
            result.Should().NotBeNull();
            result.Count().Should().BeGreaterThanOrEqualTo(2);
            Assert.NotNull(result);
            Assert.True(result.Count() >= 2);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoProductsExist_ReturnsEmptyCollection()
        {
            // Arrange //
            // Ensure the database is empty
            _context.Products.RemoveRange(_context.Products);
            await _context.SaveChangesAsync();

            // Act //
            var result = await _repository.GetAllAsync();

            // Assert //
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        // GetByAsync 
        [Fact]
        public async Task GetByAsync_WhenProductExists_ReturnsProduct()
        {
            // Arrange //
            var product = new Product { Name = "Specific Product" };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            Expression<Func<Product, bool>> predicate = p => p.Name == "Specific Product";

            // Act //
            var result = await _repository.GetByAsync(predicate);

            // Assert //
            result.Should().NotBeNull();
            result!.Id.Should().Be(product.Id);
            result.Name.Should().Be("Specific Product");
            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal("Specific Product", result.Name);
        }


        // Update Product
        [Fact]
        public async Task UpdateAsync_WhenProductExists_ReturnsSuccessResponse()
        {
            // Arrange //
            var product = new Product { Name = "Product to Update" };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            product.Name = "Updated Product Name";

            // Act //
            var response = await _repository.UpdateAsync(product);

            // Assert //
            response.Should().NotBeNull();
            response.Flag.Should().BeTrue();
            response.Message.Should().Be("Product updated successfully.");
            Assert.True(response.Flag);
            Assert.Equal("Product updated successfully.", response.Message);
        }

        [Fact]
        public async Task UpdateAsync_WhenProductDoesNotExist_ReturnsFailureResponse()
        {
            // Arrange //
            var nonExistentProduct = new Product { Id = 999, Name = "Non-existent Product" };

            // Act //
            var response = await _repository.UpdateAsync(nonExistentProduct);

            // Assert //
            response.Should().NotBeNull();
            response.Flag.Should().BeFalse();
            response.Message.Should().Be("Product not found.");
            Assert.False(response.Flag);
            Assert.Equal("Product not found.", response.Message);
        }
    }
}