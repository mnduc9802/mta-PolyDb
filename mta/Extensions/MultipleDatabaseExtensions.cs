using Microsoft.EntityFrameworkCore;
using mta.Models;

namespace mta.Extensions
{
    public static class MultipleDatabaseExtensions
    {
        public static IServiceCollection AddAndMigrateTenantDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            //Apply migrations on TenantDbContext
            using IServiceScope scopeTenant = services.BuildServiceProvider().CreateScope();
            TenantDbContext tenantDbContext = scopeTenant.ServiceProvider.GetRequiredService<TenantDbContext>();

            if (tenantDbContext.Database.GetAppliedMigrations().Any())
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Applying BaseDb Migrations.");
                Console.ResetColor();
                tenantDbContext.Database.Migrate();
            }

            //Get a list of tenants
            List<Tenant> tenantsInDb = tenantDbContext.Tenants.ToList();
            string defaultConnectionString = configuration.GetConnectionString("DefaultConnection"); //Read Default Connection string from appsettings.json

            //Loop through all tenants, apply migrations on applicationDbContext
            foreach (Tenant tenant in tenantsInDb)
            {
                string connectionString = string.IsNullOrEmpty(tenant.ConnectionString) ? defaultConnectionString : tenant.ConnectionString;

                //Application Db Context (app - per tenant)
                using IServiceScope scopeApplication = services.BuildServiceProvider().CreateScope();
                ApplicationDbContext dbContext = scopeApplication.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.SetConnectionString(connectionString);
                if (dbContext.Database.GetPendingMigrations().Any())
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Applying Migrations for '{tenant.Id}' tenant.");
                    Console.ResetColor();
                    dbContext.Database.Migrate();
                }
            }
            return services;
        }
    }
}