using Microsoft.EntityFrameworkCore;
using mta.Models;
using System;
using System.Threading.Tasks;

namespace mta.Services
{
    public class CurrentTenantService : ICurrentTenantService
    {
        private readonly TenantDbContext _context;

        public CurrentTenantService(TenantDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SetTenant(string tenantId)
        {
            var tenantInfo = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == tenantId);
            if (tenantInfo != null)
            {
                TenantId = tenantInfo.Id;
                ConnectionString = tenantInfo.ConnectionString;
                return true;
            }
            else
            {
                // Tạo mới tenant và cơ sở dữ liệu
                TenantId = tenantId;
                ConnectionString = $"Host=localhost;Database=mtaDb-mtDb-{tenantId};Username=mnduc9802;Password=123456";
                await CreateDatabaseIfNotExists(ConnectionString);

                // Thêm tenant vào cơ sở dữ liệu chung
                var newTenant = new Tenant
                {
                    Id = tenantId,
                    Name = tenantId,
                    ConnectionString = ConnectionString
                };
                _context.Tenants.Add(newTenant);
                await _context.SaveChangesAsync();

                return true;
            }
        }

        private async Task CreateDatabaseIfNotExists(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            using var context = new ApplicationDbContext(optionsBuilder.Options, this);
            await context.Database.MigrateAsync();
        }

        public string? TenantId { get; set; }
        public string? ConnectionString { get; set; }
    }
}
