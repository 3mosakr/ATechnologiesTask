using ATechnologiesTask.Api.Models;
using ATechnologiesTask.Api.Services.Abstracts;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace ATechnologiesTask.Api.Services.Implementations
{
    /// <summary>
    /// Service for handling IP-related operations, including fetching location data and validating IP addresses.
    /// </summary>
    public class IpService : IIpService
    {
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection Api_Key;

        /// <summary>
        /// Initializes a new instance of the <see cref="IpService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used for making API requests.</param>
        /// <param name="configuration">The configuration settings.</param>
        public IpService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            Api_Key = _configuration.GetSection("ApiSettings:IpGeolocationApiKey");
        }

        /// <summary>
        /// Fetches the location data for a given IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address to fetch location data for.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains the location data.</returns>
        public async Task<Location> FetchLocationFromIp(string ipAddress)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://api.ipgeolocation.io/ipgeo?apiKey={Api_Key.Value}&ip={ipAddress}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var location = JsonConvert.DeserializeObject<Location>(content);
                return location;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                // Handle API rate limit
                throw new Exception("API rate limit exceeded. Please try again later.", ex);
            }
            catch (HttpRequestException ex)
            {
                // Handle other HTTP request exceptions
                throw new Exception("An error occurred while fetching the IP location.", ex);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        /// <summary>
        /// Validates the format of an IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address to validate.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a boolean indicating whether the IP address format is valid.</returns>
        public async Task<bool> IsValidIpAsync(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return await Task.FromResult(false);

            // Regular expression for validating IPv4 and IPv6 addresses
            var ipv4Pattern = @"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            var ipv6Pattern = @"^([0-9a-fA-F]{1,4}:){7}([0-9a-fA-F]{1,4}|:)$";

            var ipv4Regex = new Regex(ipv4Pattern);
            var ipv6Regex = new Regex(ipv6Pattern);

            bool isValid = ipv4Regex.IsMatch(ipAddress) || ipv6Regex.IsMatch(ipAddress);
            return await Task.FromResult(isValid);
        }

    }
}
