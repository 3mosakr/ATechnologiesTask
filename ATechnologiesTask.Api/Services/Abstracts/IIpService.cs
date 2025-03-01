using ATechnologiesTask.Api.Models;

namespace ATechnologiesTask.Api.Services.Abstracts
{
    public interface IIpService
    {
        Task<Location> FetchLocationFromIp(string ipAddress);
        Task<bool> IsValidIpAsync(string ipAddress);

    }
}
