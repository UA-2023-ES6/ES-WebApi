using Microsoft.AspNetCore.Mvc;

namespace ES6WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeController : ControllerBase
    {
        [HttpGet("Server")]

        public async Task<ActionResult> GetServDerTimeAsync()
        {
            return Ok(new
            {
                ServerTime = DateTime.UtcNow
            });
        }

    }
}
