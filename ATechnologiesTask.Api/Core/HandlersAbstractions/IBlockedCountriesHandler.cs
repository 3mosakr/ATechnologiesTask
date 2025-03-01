using ATechnologiesTask.Api.Models;
using PropertyApplication.Core.Base;

namespace ATechnologiesTask.Api.Core.HandlersAbstractions
{
    public interface IBlockedCountriesHandler
    {
        Task<ResponseModel<Country>> BlockCountryAsync(string countryCode);
        Task<ResponseModel<Country>> UnblockCountryAsync(string countryCode);
        Task<ResponseModel<Country>> GetBlockedCountriesAsync(int page = 1, int pageSize = 10, string search = "");
        Task<ResponseModel<Country>> TemporalBlockCountryAsync(string countryCode, int durationMinutes);
        Task<bool> IsCountryBlockedAsync(string countryCode);

    }
}
