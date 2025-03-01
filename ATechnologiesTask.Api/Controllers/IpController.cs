using ATechnologiesTask.Api.Base;
using ATechnologiesTask.Api.Core.HandlersAbstractions;
using Microsoft.AspNetCore.Mvc;

namespace ATechnologiesTask.Api.Controllers
{
    /// <summary>
    /// Controller for managing IP-related operations, including IP lookup and checking if an IP is blocked.
    /// </summary>
    [ApiController]
    [Route("api/ip")]
    public class IpController : AppControllerBase
    {
        private readonly IIpHandler _ipHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="IpController"/> class.
        /// </summary>
        /// <param name="ipHandler">The handler for managing IP-related operations.</param>
        public IpController(IIpHandler ipHandler)
        {
            _ipHandler = ipHandler;
        }

        /// <summary>
        /// Looks up the location data for a given IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address to look up.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpGet("lookup")]
        public async Task<IActionResult> LookupIp([FromQuery] string ipAddress)
        {
            var location = await _ipHandler.LookupIp(ipAddress, HttpContext);
            return NewResult(location);
        }

        /// <summary>
        /// Checks if the caller's IP address is blocked.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlock()
        {
            var callerIp = HttpContext.Connection.RemoteIpAddress.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();

            var isBlocked = await _ipHandler.CheckIfIpIsBlocked(callerIp, userAgent);
            return NewResult(isBlocked);
        }

    }
}
