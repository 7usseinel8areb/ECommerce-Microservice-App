using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                var getProduct = await GetByAsync(p => p.Name!.Equals(entity.Name));

                if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                {
                    return new Response(false, $"{entity.Name} already added.");
                }

                var currentEntity = context.Products.Add(entity).Entity;

                await context.SaveChangesAsync();
                if (currentEntity is not null && currentEntity.Id > 0)
                {
                    return new Response(true, "Product created successfully.");
                }

                return new Response(false, "Failed to create product. Please try again.");
            }
            catch (Exception ex)
            {
                // Log the original exception 
                LogException.LogExceptions(ex);

                // display scary-free message to user
                return new Response(false, "An error occurred while creating the product. Please try again.");
            }
        }

        public async Task<Response> DeleteAsync(int id)
        {
            try
            {
                var product = await FindByIdAsync(id);

                if (product is null)
                {
                    return new Response(false, "Product not found.");
                }

                context.Products.Remove(product);
                await context.SaveChangesAsync();

                return new Response(true, "Product deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log the original exception 
                LogException.LogExceptions(ex);

                // display scary-free message to user
                return new Response(false, "An error occurred while deleting the product. Please try again.");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var prouct = await context.Products.FindAsync(id);

                return prouct is not null ? prouct : null!;
            }
            catch (Exception ex)
            {
                // Log the original exception 
                LogException.LogExceptions(ex);

                // display scary-free message to user
                throw new Exception("An error occurred while retriving this product. Please try again.");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                //return context.Products.AsEnumerable();
                var products = await context.Products
                    .AsNoTracking()
                    .ToListAsync();

                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                // Log the original exception 
                LogException.LogExceptions(ex);

                // display scary-free message to user
                throw new Exception("An error occurred while retriving this product. Please try again.");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> expression)
        {
            try
            {
                var product = await context.Products
                .Where(expression)
                .FirstOrDefaultAsync()!;

                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                // Log the original exception 
                LogException.LogExceptions(ex);

                // display scary-free message to user
                throw new Exception("An error occurred while retriving this product. Please try again.");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                {
                    return new Response(false, "Product not found.");
                }

                context.Entry(product).State = EntityState.Detached; // This means we are updating with a different instance
                /*
                 Added	الكيان جديد وهيتضاف في قاعدة البيانات.
                 Modified	الكيان تم تعديله وهيتحدث في قاعدة البيانات.
                 Deleted	الكيان هيتحذف من قاعدة البيانات.
                 Unchanged	الكيان متتبع ومفيش تغييرات عليه.
                 Detached	الكيان غير متتبع نهائيًا من قبل الـ context.
                 */
                context.Products.Update(entity);
                await context.SaveChangesAsync();

                return new Response(true, "Product updated successfully.");
            }
            catch (Exception ex)
            {
                // Log the original exception 
                LogException.LogExceptions(ex);

                // display scary-free message to user
                return new Response(false, "An error occurred while updating the product. Please try again.");
            }
        }
    }
}
