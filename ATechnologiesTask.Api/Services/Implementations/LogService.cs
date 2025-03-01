using ATechnologiesTask.Api.Models;
using ATechnologiesTask.Api.Services.Abstracts;
using System.Collections.Concurrent;

namespace ATechnologiesTask.Api.Services.Implementations
{
    /// <summary>
    /// Service for logging blocked attempts.
    /// </summary>
    public class LogService : ILogService
    {
        private static readonly ConcurrentBag<BlockedAttemptLog> BlockedAttemptsLogs = new ConcurrentBag<BlockedAttemptLog>();

        /// <summary>
        /// Retrieves a paginated list of blocked attempt logs.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains a list of blocked attempt logs.</returns>
        public async Task<List<BlockedAttemptLog>> GetBlockedAttemptLogsAsync(int page, int pageSize)
        {
            return await Task.Run(() =>
                BlockedAttemptsLogs.Skip((page - 1) * pageSize).Take(pageSize).ToList()
            );
        }

        /// <summary>
        /// Adds a log entry for a blocked attempt.
        /// </summary>
        /// <param name="ipAddress">The IP address of the blocked attempt.</param>
        /// <param name="countryCode">The country code associated with the blocked attempt.</param>
        /// <param name="isBlocked">Indicates whether the attempt was blocked.</param>
        /// <param name="userAgent">The user agent of the request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddBlockedAttemptLogAsync(string ipAddress, string countryCode, bool isBlocked, string userAgent)
        {
            var log = new BlockedAttemptLog
            {
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow,
                CountryCode = countryCode,
                BlockedStatus = isBlocked,
                UserAgent = userAgent
            };

            await Task.Run(() => BlockedAttemptsLogs.Add(log));
        }

    }
}
