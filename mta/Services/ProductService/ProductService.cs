using mta.Models;
using mta.Services.DTOs;

namespace mta.Services.ProductService.ProductService
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenantService;

        public ProductService(ApplicationDbContext context, ICurrentTenantService currentTenantService)
        {
            _context = context;
            _currentTenantService = currentTenantService;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            var products = _context.Products.ToList();
            return products;
        }

        public Product CreateProduct(CreateProductRequest request)
        {
            if (string.IsNullOrEmpty(_currentTenantService.TenantId))
            {
                throw new InvalidOperationException("Current tenant ID is not set.");
            }

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                TenantId = _currentTenantService.TenantId
            };

            _context.Add(product);
            _context.SaveChanges();

            return product;
        }

        public bool DeleteProduct(int id)
        {
            var product = _context.Products.Where(x => x.Id == id).FirstOrDefault();

            if (product != null)
            {
                _context.Remove(product);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
