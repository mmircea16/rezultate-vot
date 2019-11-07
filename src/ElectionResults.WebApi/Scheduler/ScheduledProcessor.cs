using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Storage;
using ElectionResults.WebApi.BackgroundService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCrontab;

namespace ElectionResults.WebApi.Scheduler
{
    public abstract class ScheduledProcessor : ScopedProcessor
    {
        private readonly IElectionConfigurationSource _electionConfigurationSource;
        private readonly ILogger<ScheduledProcessor> _logger;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;

        public ScheduledProcessor(IServiceScopeFactory serviceScopeFactory, IOptions<AppConfig> config, IElectionConfigurationSource electionConfigurationSource, ILogger<ScheduledProcessor> logger) : base(serviceScopeFactory)
        {
            _electionConfigurationSource = electionConfigurationSource;
            _logger = logger;
            _schedule = CrontabSchedule.Parse(config.Value.JobTimer);
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            _logger.LogInformation($"Next run will be at {_nextRun:F}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                var config = await _electionConfigurationSource.GetJobTimer();
                if (config.IsSuccess)
                {
                    _schedule = CrontabSchedule.Parse(config.Value);
                }
                var nextrun = _schedule.GetNextOccurrence(now);
                if (now > _nextRun)
                {
                    await Process();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                    _logger.LogInformation($"Next run will be at {_nextRun:F}");
                }
                await Task.Delay(5000, stoppingToken); //5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }
    }
}
