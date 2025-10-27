using ProductApi.Domain.Entities;

namespace ProductApi.Application.DTOs.Conversions;

public static class ProductConversions
{
    public static Product ToEntity(this ProductDTO product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        Quantity = product.Quantity
    };

    public static (ProductDTO?, IEnumerable<ProductDTO>?) FromEntity(Product product, IEnumerable<Product>? products)
    {
        // return single 
        if(product is not null || products is null)
        {
            var singleProduct = new ProductDTO(
                product.Id,
                product.Name,
                product.Price,
                product.Quantity
            );

            return (singleProduct, null);
        }

        // return multiple
        if(products is not null || product is null)
        {
            var productList = products.Select(p => new ProductDTO(
                p.Id,
                p.Name,
                p.Price,
                p.Quantity
            ));
            return (null, productList);
        }

        return (null, null);
    }
}
