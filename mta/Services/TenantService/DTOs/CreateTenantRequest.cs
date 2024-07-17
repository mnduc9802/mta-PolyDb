namespace mta.Services.TenantService.DTOs
{
    public class CreateTenantRequest
    {
        public string Name { get; set; }
        public bool Isolated { get; set; }
    }
}
