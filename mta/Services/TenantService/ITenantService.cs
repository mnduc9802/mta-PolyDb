using mta.Models;
using mta.Services.TenantService.DTOs;

namespace mta.Services.TenantService
{
    public interface ITenantService
    {
        Tenant CreateTenant (CreateTenantRequest request);
    }
}
