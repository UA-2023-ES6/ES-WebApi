using ES6WebApi.Models.Responses;
using ES6WebApi.Providers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace ES6WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class TimeController : ControllerBase
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        /// <summary>Gets the server time.</summary>
        /// <returns>The server time.</returns>
        public TimeController(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet("Server")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServerTimeResponse))]
        public async Task<ActionResult> GetServerTimeAsync()
        {
            return Ok(new ServerTimeResponse
            {
                ServerTime = _dateTimeProvider.UtcNow
            });
        }
    }
}
