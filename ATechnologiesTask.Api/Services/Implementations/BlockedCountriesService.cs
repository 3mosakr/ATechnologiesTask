using ATechnologiesTask.Api.Models;
using ATechnologiesTask.Api.Services.Abstracts;
using System.Collections.Concurrent;
using System.Globalization;

namespace ATechnologiesTask.Api.Services.Implementations
{
    /// <summary>
    /// Service for managing blocked countries, including both permanent and temporary blocks.
    /// </summary>
    public class BlockedCountriesService : IBlockedCountriesService
    {

        private readonly ConcurrentDictionary<string, Country> _blockedCountries = new ConcurrentDictionary<string, Country>();
        private readonly ConcurrentDictionary<string, DateTime> _temporalBlockedCountries = new ConcurrentDictionary<string, DateTime>();

        /// <summary>
        /// Blocks a country permanently.
        /// and if the country if temporal blocked, country will be transform from temporal to permanently. 
        /// </summary>
        /// <param name="country">The country to block.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a boolean indicating whether the country was successfully blocked.</returns>
        public Task<bool> BlockCountryAsync(Country country)
        {
            /// if the country if temporal blocked, country will be transform from temporal to permanently.
            _temporalBlockedCountries.TryRemove(country.CountryCode, out _);
            return Task.FromResult(_blockedCountries.TryAdd(country.CountryCode, country));
        }

        /// <summary>
        /// Unblocks a country (either permanently or temporarily).
        /// </summary>
        /// <param name="countryCode">The country code of the country to unblock.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains a boolean indicating whether the country was successfully unblocked.</returns>
        public Task<bool> UnblockCountryAsync(string countryCode)
        {
            // Return true if the country was removed from either collection
            return Task.FromResult(_blockedCountries.TryRemove(countryCode, out _) || _temporalBlockedCountries.TryRemove(countryCode, out _));
        }

        /// <summary>
        /// Checks if a country is blocked (either permanently or temporarily).
        /// </summary>
        /// <param name="countryCode">The country code to check.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains a boolean indicating whether the country is blocked.</returns>
        public Task<bool> IsCountryBlockedAsync(string countryCode)
        {
            // Check if the country is permanently blocked or temporarily blocked
            if (_blockedCountries.ContainsKey(countryCode) || _temporalBlockedCountries.ContainsKey(countryCode))
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Gets a list of all blocked countries (both permanent and temporary).
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains a list of blocked countries.</returns>
        public Task<List<Country>> GetBlockedCountriesAsync()
        {
            var blockedCountries = _blockedCountries.Values.ToList();

            // Add temporarily blocked countries
            foreach (var countryCode in _temporalBlockedCountries.Keys)
            {
                RegionInfo myRI = new RegionInfo(countryCode);
                var country = new Country { CountryCode = myRI.Name, CountryName = myRI.EnglishName };
                blockedCountries.Add(country);
            }

            return Task.FromResult(blockedCountries);
        }

        /// <summary>
        /// Temporarily blocks a country for a specified duration.
        /// </summary>
        /// <param name="countryCode">The country code of the country to block.</param>
        /// <param name="durationMinutes">The duration in minutes for which the country should be blocked.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a boolean indicating whether the country was successfully blocked temporarily.</returns>
        public async Task<bool> TemporalBlockCountryAsync(string countryCode, int durationMinutes)
        {
            /// check if country is already blocked
            bool IsCountryBlocked = await IsCountryBlockedAsync(countryCode);
            if (IsCountryBlocked)
                return false;
            var expiration = DateTime.UtcNow.AddMinutes(durationMinutes);
            return _temporalBlockedCountries.TryAdd(countryCode, expiration);
        }


        /// <summary>
        /// Removes expired temporary blocks.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task RemoveExpiredTemporalBlocksAsync()
        {
            var now = DateTime.UtcNow;
            foreach (var country in _temporalBlockedCountries.Keys.ToList())
            {
                if (_temporalBlockedCountries[country] < now)
                {
                    _temporalBlockedCountries.TryRemove(country, out _);
                }
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the count of all blocked countries (both permanent and temporary).
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains the count of blocked countries.</returns>
        public Task<int> BlockedCountriesCountAsync()
        {
            return Task.FromResult(_blockedCountries.Count + _temporalBlockedCountries.Count);
        }
    }
}
