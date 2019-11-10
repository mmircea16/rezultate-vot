using System;
using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Services.CsvDownload;
using ElectionResults.Core.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElectionResults.WebApi.Scheduler
{
    public class ScheduleTask : ScheduledProcessor
    {
        private readonly ICsvDownloaderJob _csvDownloaderJob;
        private readonly ILogger<ScheduleTask> _logger;

        public ScheduleTask(IServiceScopeFactory serviceScopeFactory,
            ICsvDownloaderJob csvDownloaderJob,
            ILogger<ScheduleTask> logger,
            IElectionConfigurationSource electionConfigurationSource,
            IOptions<AppConfig> config)
            : base(serviceScopeFactory, config, electionConfigurationSource, logger)
        {
            _csvDownloaderJob = csvDownloaderJob;
            _logger = logger;
        }

        public override async Task ProcessInScope(IServiceProvider serviceProvider)
        {
            Console.WriteLine($"Processing starts here at {DateTime.UtcNow:F}");
            await _csvDownloaderJob.DownloadFiles();
            _logger.LogInformation("Sending live results");
        }
    }
}