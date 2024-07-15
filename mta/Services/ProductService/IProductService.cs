using mta.Models;
using mta.Services.DTOs;

namespace mta.Services.ProductService.ProductService
{
    public interface IProductService
    {
        IEnumerable<Product> GetAllProducts();
        Product CreateProduct(CreateProductRequest request);
        bool DeleteProduct(int id);
    }
}
