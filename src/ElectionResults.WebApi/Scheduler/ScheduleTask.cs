using System;
using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Services.CsvDownload;
using ElectionResults.Core.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ElectionResults.WebApi.Scheduler
{
    public class ScheduleTask : ScheduledProcessor
    {
        private readonly ICsvDownloaderJob _csvDownloaderJob;
        public ScheduleTask(IServiceScopeFactory serviceScopeFactory,
            ICsvDownloaderJob csvDownloaderJob,
            IElectionConfigurationSource electionConfigurationSource,
            IOptions<AppConfig> config)
            : base(serviceScopeFactory, config, electionConfigurationSource)
        {
            _csvDownloaderJob = csvDownloaderJob;
        }

        public override async Task ProcessInScope(IServiceProvider serviceProvider)
        {
            Console.WriteLine($"Processing starts here at {DateTime.UtcNow:F}");
            await _csvDownloaderJob.DownloadFiles();
        }
    }
}