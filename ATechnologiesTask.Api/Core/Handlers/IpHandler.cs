using ATechnologiesTask.Api.Core.HandlersAbstractions;
using ATechnologiesTask.Api.Models;
using ATechnologiesTask.Api.Services.Abstracts;
using PropertyApplication.Core.Base;

namespace ATechnologiesTask.Api.Core.Handlers
{
    /// <summary>
    /// Handler for managing IP-related operations, including IP lookup and checking if an IP is blocked.
    /// </summary>
    public class IpHandler : IIpHandler
    {
        private readonly IIpService _ipService;
        private readonly IBlockedCountriesService _blockedCountriesService;
        private readonly ILogService _logService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IpHandler"/> class.
        /// </summary>
        /// <param name="ipService">The service for handling IP-related operations.</param>
        /// <param name="blockedCountriesService">The service for managing blocked countries.</param>
        /// <param name="logService">The service for logging blocked attempts.</param>
        public IpHandler(IIpService ipService, IBlockedCountriesService blockedCountriesService, ILogService logService)
        {
            _ipService = ipService;
            _blockedCountriesService = blockedCountriesService;
            _logService = logService;
        }

        /// <summary>
        /// Looks up the location data for a given IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address to look up.</param>
        /// <param name="httpContext">The HTTP context to get the caller's IP address if <paramref name="ipAddress"/> is null or empty.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a response model with the location data.</returns>
        public async Task<ResponseModel<Location>> LookupIp(string ipAddress, HttpContext httpContext)
        {
            // Validate IP address format
            if (!string.IsNullOrEmpty(ipAddress) && !await _ipService.IsValidIpAsync(ipAddress))
            {
                return new ResponseModel<Location>("Invalid IP address format.", false);
            }

            // If ipAddress is null or empty, use the caller's IP address
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            }

            try
            {
                var data = await _ipService.FetchLocationFromIp(ipAddress);
                return new ResponseModel<Location>([data], $"Get data successfully for Ip: {ipAddress}");
            }
            catch (Exception ex)
            {
                return new ResponseModel<Location>("There is an error occured will getting data", false, [ex.Message]);
            }
        }

        /// <summary>
        /// Checks if a given IP address is blocked.
        /// </summary>
        /// <param name="ipAddress">The IP address to check.</param>
        /// <param name="userAgent">The user agent of the request.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains a response model indicating whether the IP address is blocked.</returns>
        public async Task<ResponseModel<bool>> CheckIfIpIsBlocked(string ipAddress, string userAgent)
        {
            try
            {
                var location = await _ipService.FetchLocationFromIp(ipAddress);
                // Check if country is blocked or temporarily blocked
                var isBlocked = await _blockedCountriesService.IsCountryBlockedAsync(location.Country_Code2);

                // Log the attempt
                if (isBlocked)
                {
                    await _logService.AddBlockedAttemptLogAsync(ipAddress, location.Country_Code2, isBlocked, userAgent);
                    return new ResponseModel<bool>("You Can't Access this site from your country right now.", false);
                }
                return new ResponseModel<bool>([!isBlocked], "You can access this site from your country.");
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>("There is an error occured will getting data", false, [ex.Message]);
            }
        }

    }
}
