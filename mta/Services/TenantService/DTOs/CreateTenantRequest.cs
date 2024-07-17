namespace mta.Services.TenantService.DTOs
{
    public class CreateTenantRequest
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public bool Isolated { get; set; }
    }
}
