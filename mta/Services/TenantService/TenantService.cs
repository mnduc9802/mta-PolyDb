using Microsoft.EntityFrameworkCore;
using mta.Models;
using mta.Services.TenantService.DTOs;


namespace mta.Services.TenantService
{
    public class TenantService : ITenantService
    {
        private readonly TenantDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public TenantService(TenantDbContext context, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _context = context;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public Tenant CreateTenant(CreateTenantRequest request)
        {
            string newConnectionString = null;
            if (request.Isolated == true)
            {
                // Generate a connection string for new tenant database
                string dbName = "mtaDb-mtDb-" + request.Id; // Ensure the correct prefix
                string defaultConnectionString = _configuration.GetConnectionString("DefaultConnection");
                newConnectionString = defaultConnectionString.Replace("mtaDb-mtDb", dbName);

                // Log new connection string
                Console.WriteLine($"Creating database with connection string: {newConnectionString}");

                // Create a new tenant database and apply migrations
                try
                {
                    using IServiceScope scopeTenant = _serviceProvider.CreateScope();
                    ApplicationDbContext dbContext = scopeTenant.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.SetConnectionString(newConnectionString);

                    // Check if database exists and apply migrations
                    if (!dbContext.Database.GetAppliedMigrations().Any())
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"Applying migrations for new '{request.Id}' tenant.");
                        Console.ResetColor();
                        dbContext.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while creating the tenant database: " + ex.Message);
                }
            }

            Tenant tenant = new()
            {
                Id = request.Id,
                Name = request.Name,
                ConnectionString = newConnectionString,
            };

            _context.Add(tenant);
            _context.SaveChanges();

            return tenant;
        }
    }
}
