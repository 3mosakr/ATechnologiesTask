using ATechnologiesTask.Api.Models;

namespace ATechnologiesTask.Api.Services.Abstracts
{
    public interface ILogService
    {
        
        Task<List<BlockedAttemptLog>> GetBlockedAttemptLogsAsync(int page, int pageSize);
        Task AddBlockedAttemptLogAsync(string ipAddress, string countryCode, bool isBlocked, string userAgent);

    }
}
