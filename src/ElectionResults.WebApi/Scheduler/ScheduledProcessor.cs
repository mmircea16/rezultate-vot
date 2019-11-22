using System;
using System.Threading;
using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Storage;
using ElectionResults.WebApi.BackgroundService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElectionResults.WebApi.Scheduler
{
    public abstract class ScheduledProcessor : ScopedProcessor
    {
        private readonly IElectionConfigurationSource _electionConfigurationSource;
        private readonly ILogger<ScheduledProcessor> _logger;
        private DateTime _nextRun;
        private int _intervalInSeconds;

        public ScheduledProcessor(IServiceScopeFactory serviceScopeFactory, IOptions<AppConfig> config, IElectionConfigurationSource electionConfigurationSource, ILogger<ScheduledProcessor> logger) : base(serviceScopeFactory)
        {
            _electionConfigurationSource = electionConfigurationSource;
            _logger = logger;
            _intervalInSeconds = config.Value.IntervalInSeconds;
            _nextRun = DateTime.Now.AddSeconds(_intervalInSeconds);
            _logger.LogInformation($"Next run will be at {_nextRun:F}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                var config = await _electionConfigurationSource.GetInterval();
                if (config.IsSuccess)
                {
                    _intervalInSeconds = config.Value;
                }
                if (now > _nextRun)
                {
                    await Process();
                    _nextRun = DateTime.Now.AddSeconds(_intervalInSeconds);
                    _logger.LogInformation($"Next run will be at {_nextRun:F}");
                }
            }
            while (!stoppingToken.IsCancellationRequested);
        }
    }
}
