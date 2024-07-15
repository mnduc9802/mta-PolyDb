using Microsoft.EntityFrameworkCore;
using mta.Services;

namespace mta.Models
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentTenantService _currentTenantService;
        public string CurrentTenantId { get; set; }
        public string CurrentTenantConnectionString { get; set; }

        //Constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentTenantService currentTenantService) : base(options)
        { 
            _currentTenantService = currentTenantService;
            CurrentTenantId = _currentTenantService.TenantId;
            CurrentTenantConnectionString = _currentTenantService.ConnectionString;
        }

        //DbSet
        public DbSet<Product> Products { get; set; }

        //On App Startup
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>().HasQueryFilter(a => a.TenantId == CurrentTenantId);
        }

        //On Configuring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string tenantConnectionString = CurrentTenantConnectionString;
            if (!string.IsNullOrEmpty(tenantConnectionString)) // use tenant db if one is specified
            {
                _ = optionsBuilder.UseNpgsql(tenantConnectionString);
            }
        }

        //Save Changes
        public override int SaveChanges()
        {
            if (string.IsNullOrEmpty(CurrentTenantId))
            {
                throw new InvalidOperationException("Current tenant ID is not set.");
            }

            foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                        entry.Entity.TenantId = CurrentTenantId;
                        break;
                }
            }

            var result = base.SaveChanges();
            return result;
        }
    }
}
