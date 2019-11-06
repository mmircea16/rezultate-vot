using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ElectionResults.Core.Storage;
using ElectionResults.WebApi.BackgroundService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NCrontab;

namespace ElectionResults.WebApi.Scheduler
{
    public abstract class ScheduledProcessor : ScopedProcessor
    {
        private readonly CrontabSchedule _schedule;
        private DateTime _nextRun;

        public ScheduledProcessor(IServiceScopeFactory serviceScopeFactory, IOptions<AppConfig> config) : base(serviceScopeFactory)
        {
            _schedule = CrontabSchedule.Parse(config.Value.JobTimer);
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            _nextRun = DateTime.Now.AddMilliseconds(1500);
            Console.WriteLine($"Next run will be at {_nextRun:F}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                var nextrun = _schedule.GetNextOccurrence(now);
                if (now > _nextRun)
                {
                    await Process();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }
                await Task.Delay(5000, stoppingToken); //5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }
    }
}
