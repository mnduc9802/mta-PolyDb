using Microsoft.EntityFrameworkCore;

namespace mta.Models
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options) 
        { 
            
        }

        public DbSet<Tenant> Tenants { get; set; }
    }
}
