using ATechnologiesTask.Api.Base;
using ATechnologiesTask.Api.Core.HandlersAbstractions;
using Microsoft.AspNetCore.Mvc;

namespace ATechnologiesTask.Api.Controllers
{
    /// <summary>
    /// Controller for managing log-related operations, including retrieving blocked attempt logs.
    /// </summary>
    [ApiController]
    [Route("api/logs")]
    public class LogsController : AppControllerBase
    {
        private readonly ILogsHandler _logsHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogsController"/> class.
        /// </summary>
        /// <param name="logsHandler">The handler for managing log-related operations.</param>
        public LogsController(ILogsHandler logsHandler)
        {
            _logsHandler = logsHandler;
        }

        /// <summary>
        /// Retrieves a paginated list of blocked attempt logs.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpGet("blocked-attempts")]
        public async Task<IActionResult> GetBlockedAttempts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedLogs = await _logsHandler.GetBlockedAttemptsAsync(page, pageSize);
            return NewResult(pagedLogs);
        }
    }
}