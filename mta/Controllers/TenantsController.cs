using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mta.Services.TenantService;
using mta.Services.TenantService.DTOs;


namespace mta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpPost]
        public IActionResult Post(CreateTenantRequest request)
        {
            try
            {
                var result = _tenantService.CreateTenant(request);
                return Ok(result);
            }
            catch (DuplicateKeyException ex)
            {
                return Conflict(new { message = ex.Message }); // Trả về lỗi 409 Conflict nếu Key đã tồn tại
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error" }); // Xử lý các ngoại lệ khác
            }
        }
    }
}
