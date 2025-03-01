using ATechnologiesTask.Api.Helpers;
using ATechnologiesTask.Api.Services.Abstracts;

namespace ATechnologiesTask.Api.Services
{
    /// <summary>
    /// Background service for periodically removing expired temporary blocks from the blocked countries list.
    /// </summary>
    public class BackgroundService : IHostedService
    {

        private readonly IBlockedCountriesService _blockedCountriesService;
        private Timer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundService"/> class.
        /// </summary>
        /// <param name="blockedCountriesService">The service for managing blocked countries.</param>
        public BackgroundService(IBlockedCountriesService blockedCountriesService)
        {
            _blockedCountriesService = blockedCountriesService;
        }

        /// <summary>
        /// Starts the background service.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                RemoveExpiredTemporalBlocksAsync,
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(Constants.BackgroundPeriodInMinutes)
                );
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the background service.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes expired temporary blocks from the blocked countries list.
        /// </summary>
        /// <param name="state">An object that contains information to be used by the callback method.</param>
        private async void RemoveExpiredTemporalBlocksAsync(object state)
        {
            await _blockedCountriesService.RemoveExpiredTemporalBlocksAsync();
        }
    }
}
