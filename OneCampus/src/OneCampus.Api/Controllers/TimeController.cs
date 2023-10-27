using Microsoft.AspNetCore.Mvc;
using OneCampus.Application.Providers;
using OneCampus.Models.Responses;
using System.Net.Mime;

namespace OneCampus.Controllers
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
        public ActionResult GetServerTime()
        {
            return Ok(new ServerTimeResponse
            {
                ServerTime = _dateTimeProvider.UtcNow
            });
        }
    }
}
