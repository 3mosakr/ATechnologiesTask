using ATechnologiesTask.Api.Base;
using ATechnologiesTask.Api.Core.HandlersAbstractions;
using ATechnologiesTask.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ATechnologiesTask.Api.Controllers
{
    /// <summary>
    /// Controller for managing blocked countries, including both permanent and temporary blocks.
    /// </summary>
    [ApiController]
    [Route("api/countries")]
    public class CountriesController : AppControllerBase
    {
        private readonly IBlockedCountriesHandler _blockedCountriesHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountriesController"/> class.
        /// </summary>
        /// <param name="blockedCountriesHandler">The handler for managing blocked countries.</param>
        public CountriesController(IBlockedCountriesHandler blockedCountriesHandler)
        {
            _blockedCountriesHandler = blockedCountriesHandler;
        }

        /// <summary>
        /// Blocks a country permanently.
        /// </summary>
        /// <param name="countryCode">The country code of the country to block.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpPost("block")]
        public async Task<IActionResult> BlockCountry([FromBody] string countryCode)
        {
            var response = await _blockedCountriesHandler.BlockCountryAsync(countryCode);
            return NewResult(response);
        }

        /// <summary>
        /// Unblocks a country.
        /// </summary>
        /// <param name="countryCode">The country code of the country to unblock.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpDelete("block/{countryCode}")]
        public async Task<IActionResult> UnblockCountry(string countryCode)
        {
            var response = await _blockedCountriesHandler.UnblockCountryAsync(countryCode);
            return NewResult(response);
        }

        /// <summary>
        /// Gets a paginated list of blocked countries with optional search.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="search">The search term to filter countries.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpGet("blocked")]
        public async Task<IActionResult> GetBlockedCountries([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = "")
        {
            var response = await _blockedCountriesHandler.GetBlockedCountriesAsync(page, pageSize, search);
            return NewResult(response);
        }

        /// <summary>
        /// Temporarily blocks a country for a specified duration.
        /// </summary>
        /// <param name="request">The request containing the country code and duration in minutes.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpPost("temporal-block")]
        public async Task<IActionResult> TemporalBlockCountry([FromBody] TemporalBlockRequest request)
        {
            var response = await _blockedCountriesHandler.TemporalBlockCountryAsync(request.CountryCode, request.DurationMinutes);
            return NewResult(response);
        }
    }

}