using ATechnologiesTask.Api.Core.HandlersAbstractions;
using ATechnologiesTask.Api.Models;
using ATechnologiesTask.Api.Services.Abstracts;
using PropertyApplication.Core.Base;

namespace ATechnologiesTask.Api.Core.Handlers
{
    /// <summary>
    /// Handler for managing log-related operations, including retrieving blocked attempt logs.
    /// </summary>
    public class LogsHandler : ILogsHandler
    {
        private readonly ILogService _logService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogsHandler"/> class.
        /// </summary>
        /// <param name="logService">The service for managing logs.</param>
        public LogsHandler(ILogService logService)
        {
            _logService = logService;
        }

        /// <summary>
        /// Retrieves a paginated list of blocked attempt logs.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a response model with the blocked attempt logs.</returns>
        public async Task<ResponseModel<BlockedAttemptLog>> GetBlockedAttemptsAsync(int page, int pageSize)
        {
            try
            {
                var data = await _logService.GetBlockedAttemptLogsAsync(page, pageSize);
                var count = data.Count;

                return new PaginatedResponse<BlockedAttemptLog>(data, "Get Blocked Attempt Logs", count, page, pageSize);
            }
            catch (Exception ex)
            {
                return new ResponseModel<BlockedAttemptLog>("There is an error occured will getting data", false, [ex.Message]);

            }

        }

    }
}
