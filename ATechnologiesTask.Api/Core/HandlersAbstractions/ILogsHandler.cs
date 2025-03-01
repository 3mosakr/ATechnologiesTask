using ATechnologiesTask.Api.Models;
using PropertyApplication.Core.Base;

namespace ATechnologiesTask.Api.Core.HandlersAbstractions
{
    public interface ILogsHandler
    {
        Task<ResponseModel<BlockedAttemptLog>> GetBlockedAttemptsAsync(int page = 1, int pageSize = 10);
    }
}
