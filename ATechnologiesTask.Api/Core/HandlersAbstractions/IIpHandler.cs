using ATechnologiesTask.Api.Models;
using PropertyApplication.Core.Base;

namespace ATechnologiesTask.Api.Core.HandlersAbstractions
{
    public interface IIpHandler
    {
        Task<ResponseModel<Location>> LookupIp(string ipAddress, HttpContext httpContext);
        Task<ResponseModel<bool>> CheckIfIpIsBlocked(string ipAddress, string userAgent);
    }
}
