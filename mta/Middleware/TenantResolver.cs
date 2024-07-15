using mta.Services;

namespace mta.Middleware
{
    public class TenantResolver
    {
        private readonly RequestDelegate _next;

        public TenantResolver(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICurrentTenantService currentTenantService)
        {
            context.Request.Headers.TryGetValue("X-Tenant", out var tenantFromHeader);
            if (string.IsNullOrEmpty(tenantFromHeader) == false)
            {
               await currentTenantService.SetTenant(tenantFromHeader);
            } 
            await _next(context);    
        }
    }
}
