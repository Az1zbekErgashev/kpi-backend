using Microsoft.AspNetCore.Mvc;

namespace Kpi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestTimeController : ControllerBase
    {
        [HttpGet("now")]
        public IActionResult Get()
        {
            return Ok(new
            {
                UtcNow = DateTime.UtcNow,
                ServerNow = DateTime.Now,
                CreatedAt = DateTime.UtcNow 
            });
        }
    }
}
