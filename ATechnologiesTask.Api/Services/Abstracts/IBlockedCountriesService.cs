using ATechnologiesTask.Api.Models;

namespace ATechnologiesTask.Api.Services.Abstracts
{
    public interface IBlockedCountriesService
    {
        Task<bool> BlockCountryAsync(Country country);
        Task<bool> UnblockCountryAsync(string countryCode);
        Task<bool> IsCountryBlockedAsync(string countryCode);
        Task<List<Country>> GetBlockedCountriesAsync();
        Task<bool> TemporalBlockCountryAsync(string countryCode, int durationMinutes);
        Task RemoveExpiredTemporalBlocksAsync();
        Task<int> BlockedCountriesCountAsync();
    }
}
