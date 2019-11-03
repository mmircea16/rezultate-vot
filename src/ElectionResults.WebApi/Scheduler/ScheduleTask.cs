using System;
using System.Threading.Tasks;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services;
using ElectionResults.Core.Services.CsvDownload;
using ElectionResults.Core.Storage;
using ElectionResults.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ElectionResults.WebApi.Scheduler
{
    public class ScheduleTask : ScheduledProcessor
    {
        private readonly ICsvDownloaderJob _csvDownloaderJob;
        private readonly IHubContext<ElectionResultsHub> _hubContext;
        private readonly IResultsAggregator _resultsAggregator;

        public ScheduleTask(IServiceScopeFactory serviceScopeFactory,
            ICsvDownloaderJob csvDownloaderJob,
            IHubContext<ElectionResultsHub> hubContext,
            IResultsAggregator resultsAggregator,
            IOptions<AppConfig> config)
            : base(serviceScopeFactory, config)
        {
            _csvDownloaderJob = csvDownloaderJob;
            _hubContext = hubContext;
            _resultsAggregator = resultsAggregator;
        }

        public override async Task ProcessInScope(IServiceProvider serviceProvider)
        {
            Console.WriteLine($"Processing starts here at {DateTime.UtcNow:F}");    
            await _csvDownloaderJob.DownloadFiles();
            var provisionalResults = await _resultsAggregator.GetResults(ResultsType.Provisional);
            var partialResults = await _resultsAggregator.GetResults(ResultsType.Partial);
            var finalResults = await _resultsAggregator.GetResults(ResultsType.Final);
            await _hubContext.Clients.All.SendCoreAsync("results-updated", new[] { provisionalResults, partialResults, finalResults });
        }
    }
}