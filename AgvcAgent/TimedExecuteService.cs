using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AgvcAgent
{
    public class TimedExecuteService : BackgroundService
    {
        private readonly ILogger<TimedExecuteService> _logger;
        private readonly TimedExecuteServiceSettings _settings;

        public TimedExecuteService(IOptions<TimedExecuteServiceSettings> settings, ILogger<TimedExecuteService> logger)
        {
            //Constructor’s parameters validations   
            _settings = settings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"TimedExecutService is starting.");
            stoppingToken.Register(() => _logger.LogDebug($"TimedExecutService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"TimedExecutService doing background work.");
                //Doing Somethings
                await Task.Delay(_settings.CheckUpdateTime, stoppingToken);
            }

            _logger.LogDebug($"TimedExecutService is stopping.");
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            // Doing Somethings
        }
    }
}