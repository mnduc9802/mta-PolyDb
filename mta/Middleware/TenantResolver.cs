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
            context.Request.Headers.TryGetValue("Key", out var keyFromHeader);
            if (string.IsNullOrEmpty(tenantFromHeader) == false && string.IsNullOrEmpty(keyFromHeader) == false)
            {
                await currentTenantService.SetTenant(tenantFromHeader, keyFromHeader);
            } 
            await _next(context);    
        }
    }
}
