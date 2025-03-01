using ATechnologiesTask.Api.Core.HandlersAbstractions;
using ATechnologiesTask.Api.Helpers;
using ATechnologiesTask.Api.Models;
using ATechnologiesTask.Api.Services.Abstracts;
using PropertyApplication.Core.Base;
using System.Globalization;
using System.Net;

namespace ATechnologiesTask.Api.Core.Handlers
{
    /// <summary>
    /// Handler for managing blocked countries, including both permanent and temporary blocks.
    /// </summary>
    public class BlockedCountriesHandler : IBlockedCountriesHandler
    {
        private readonly IBlockedCountriesService _blockedCountriesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockedCountriesHandler"/> class.
        /// </summary>
        /// <param name="blockedCountriesService">The service for managing blocked countries.</param>
        public BlockedCountriesHandler(IBlockedCountriesService blockedCountriesService)
        {
            _blockedCountriesService = blockedCountriesService;
        }

        /// <summary>
        /// Blocks a country permanently.
        /// If the country if temporal blocked, country will be transform from temporal to permanently. 
        /// </summary>
        /// <param name="countryCode">The country code of the country to block.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains a response model with the blocked country.
        /// It return ResponseModel property status: true and country information in Data property if country blocked successfully, otherwise status: false with the message. </returns>
        public async Task<ResponseModel<Country>> BlockCountryAsync(string countryCode)
        {
            /// Validate the countryCode format.
            if (!IsCountryCodeValidFormat(countryCode))
            {
                return new ResponseModel<Country>("Invalid country code format.", false);
            }

            try
            {
                /// Use RegionInfo class to get the country data if it valid country code.
                ///  if country code is not valid it will throw ArgumentException
                RegionInfo myRI = new RegionInfo(countryCode);
                /// Create country object to add to blocked countries
                var country = new Country { CountryCode = myRI.Name, CountryName = myRI.EnglishName };
                /// Try to block the country if it not already blocked
                /// if it already blocked return response with Conflict status code 
                if (!await _blockedCountriesService.BlockCountryAsync(country))
                {
                    return new ResponseModel<Country>()
                    {
                        Status = false,
                        StatusCode = HttpStatusCode.Conflict,
                        Message = $"Country: '{country.CountryName}' is already blocked.",
                    };
                }
                return new ResponseModel<Country>([country], $"Country: '{country.CountryName}' is blocked successfully.");
            }
            catch (Exception ex)
            {
                return new ResponseModel<Country>("Invalid country code.", false, [ex.Message]);
            }
        }

        /// <summary>
        /// Unblocks a country.
        /// </summary>
        /// <param name="countryCode">The country code of the country to unblock.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a response model with the unblocked country.
        /// It return ResponseModel property status: true if country Unblocked successfully, otherwise status: false with the message.</returns>
        public async Task<ResponseModel<Country>> UnblockCountryAsync(string countryCode)
        {
            /// Validate the countryCode format.
            if (!IsCountryCodeValidFormat(countryCode))
            {
                return new ResponseModel<Country>("Invalid country code format.", false);
            }
            try
            {
                /// Use RegionInfo class to get the country data if it valid country code.
                ///  if country code is not valid it will throw ArgumentException
                RegionInfo myRI = new RegionInfo(countryCode);
                /// Try to unblock the country if it blocked
                /// if it already not blocked return response with NotFound status code 
                if (!await _blockedCountriesService.UnblockCountryAsync(myRI.Name))
                {
                    return new ResponseModel<Country>()
                    {
                        Status = false,
                        StatusCode = HttpStatusCode.NotFound,
                        Message = $"Country: '{myRI.EnglishName}' not found in blocked list.",
                    };
                }
                return new ResponseModel<Country>($"Country: '{myRI.EnglishName}' is unblocked.");
            }
            catch (Exception ex)
            {
                return new ResponseModel<Country>("Invalid country code.", false, [ex.Message]);
            }
        }

        /// <summary>
        /// Gets a paginated list of blocked countries with optional search.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="search">The search term to filter countries.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains a paginated response model with the blocked countries.</returns>
        public async Task<ResponseModel<Country>> GetBlockedCountriesAsync(int page, int pageSize, string search)
        {
            try
            {
                /// Retrieve the blocked countries list
                var blockedCountries = await _blockedCountriesService.GetBlockedCountriesAsync();
                /// create Query for the data
                var blockedCountriesQuery = blockedCountries.AsQueryable();
                /// Filter data based on Search with Country Name or Country Code
                if (!string.IsNullOrEmpty(search))
                {
                    blockedCountriesQuery = blockedCountriesQuery.Where(c =>
                        c.CountryCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        c.CountryName.Contains(search, StringComparison.OrdinalIgnoreCase));
                }
                /// calculate the length of data (use in pagination)
                var totalCount = blockedCountriesQuery.Count();
                /// paginate the data
                var paginatedBlockedCountries = blockedCountriesQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                /// return paginated data 
                return new PaginatedResponse<Country>(paginatedBlockedCountries, $"Countries with search: '{search}'",
                                                        totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                /// if any exception occurred while proccessing the data return resonse with Status: false and status code: InternalServerError
                return new ResponseModel<Country>()
                {
                    Status = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "An error occurred while retrieving blocked countries.",
                    Errors = [ex.Message]
                };
            }
        }

        /// <summary>
        /// Temporarily blocks a country for a specified duration.
        /// </summary>
        /// <param name="countryCode">The country code of the country to block.</param>
        /// <param name="durationMinutes">The duration in minutes for which the country should be blocked. The default duration = (2 hours).</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains a response model with the temporarily blocked country.
        /// It return ResponseModel property status: true if country Temporal blocked successfully, otherwise status: false with the message.</returns>
        public async Task<ResponseModel<Country>> TemporalBlockCountryAsync(string countryCode, int durationMinutes)
        {
            /// Validate the countryCode format.
            if (!IsCountryCodeValidFormat(countryCode))
            {
                return new ResponseModel<Country>("Invalid country code format.", false);
            }
            try
            {
                /// Use RegionInfo class to get the country data if it valid country code.
                ///  if country code is not valid it will throw ArgumentException
                RegionInfo myRI = new RegionInfo(countryCode);
                /// if durationMinutes = 0 set it to default value (120 Minutes) 
                durationMinutes = durationMinutes == 0 ? Constants.DefaultDurationMinutes : durationMinutes;
                /// validate the durationMinutes to be in the available range [1, MaxDurationMinutes]
                if (durationMinutes < 1 || durationMinutes > Constants.MaxDurationMinutes)
                    return new ResponseModel<Country>($"The Duration minutes must be between 1 and {Constants.MaxDurationMinutes} (24 hours)", false);

                /// Try to block the country if it not already blocked
                /// if it already blocked return response with Conflict status code
                if (!await _blockedCountriesService.TemporalBlockCountryAsync(myRI.Name, durationMinutes))
                {
                    return new ResponseModel<Country>()
                    {
                        Status = false,
                        StatusCode = HttpStatusCode.Conflict,
                        Message = $"Country: '{myRI.EnglishName}' is already blocked.",
                    };
                }
                return new ResponseModel<Country>($"Country: '{myRI.EnglishName}' is temporarily blocked successfully for {durationMinutes} Minutes.");
            }
            catch (Exception ex)
            {
                return new ResponseModel<Country>("Invalid country code.", false, [ex.Message]);
            }
        }

        /// <summary>
        /// Checks if a country is blocked (either permanently or temporarily).
        /// </summary>
        /// <param name="countryCode">The country code to check.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains a boolean indicating whether the country is blocked.</returns>
        public async Task<bool> IsCountryBlockedAsync(string countryCode)
        {
            return await _blockedCountriesService.IsCountryBlockedAsync(countryCode);
        }

        /// <summary>
        /// Validates the format of a country code.
        /// </summary>
        /// <param name="countryCode">The country code to validate.</param>
        /// <returns>A boolean indicating whether the country code format is valid.</returns>
        private bool IsCountryCodeValidFormat(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
            {
                return false;
            }
            return true;
        }
    }
}
