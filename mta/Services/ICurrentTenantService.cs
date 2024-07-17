namespace mta.Services
{
    public interface ICurrentTenantService
    {
        string? TenantId { get; set; }
        public string? ConnectionString { get; set; }
        public Task<bool> SetTenant(string tenant, string key);
    }
}
