using mta.Models;
using mta.Services.DTOs;
using mta.Services.TenantService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using mta.Services.ProductService.ProductService;
using mta.Services.TenantService.DTOs;

namespace mta.Services.ProductService.ProductService
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly TenantDbContext _tenantDbContext;
        private readonly ITenantService _tenantService;
        private readonly ICurrentTenantService _currentTenantService;

        public ProductService(
            ApplicationDbContext context,
            TenantDbContext tenantDbContext,
            ITenantService tenantService,
            ICurrentTenantService currentTenantService)
        {
            _context = context;
            _tenantDbContext = tenantDbContext;
            _tenantService = tenantService;
            _currentTenantService = currentTenantService;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product CreateProduct(CreateProductRequest request)
        {
            var tenantId = _currentTenantService.TenantId;

            if (string.IsNullOrEmpty(tenantId))
            {
                throw new InvalidOperationException("Current tenant ID is not set.");
            }

            var tenant = _tenantDbContext.Tenants.SingleOrDefault(t => t.Id == tenantId);
            if (tenant == null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Tenant '{tenantId}' chưa từng tồn tại. Hệ thống sẽ tự tạo tenant với tên '{tenantId}'.");

                var createTenantRequest = new CreateTenantRequest
                {
                    Name = tenantId,
                    Key = "DefaultKey", // Cung cấp key mặc định nếu cần
                    Isolated = true
                };

                tenant = _tenantService.CreateTenant(createTenantRequest);
                _currentTenantService.SetTenant(tenantId, createTenantRequest.Key).Wait(); // Đảm bảo SetTenant được gọi đồng bộ
                Console.ResetColor();
            }

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                TenantId = tenant.Id
            };

            _context.Add(product);
            _context.SaveChanges();

            return product;
        }

        public bool DeleteProduct(int id)
        {
            var product = _context.Products.SingleOrDefault(x => x.Id == id);

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
