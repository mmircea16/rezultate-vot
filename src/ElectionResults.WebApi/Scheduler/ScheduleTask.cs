using System;
using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services;
using ElectionResults.Core.Services.CsvDownload;
using ElectionResults.Core.Storage;
using ElectionResults.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElectionResults.WebApi.Scheduler
{
    public class ScheduleTask : ScheduledProcessor
    {
        private readonly ICsvDownloaderJob _csvDownloaderJob;
        private readonly IHubContext<ElectionResultsHub> _hubContext;
        private readonly IResultsAggregator _resultsAggregator;
        private readonly ILogger<ScheduleTask> _logger;

        public ScheduleTask(IServiceScopeFactory serviceScopeFactory,
            ICsvDownloaderJob csvDownloaderJob,
            IHubContext<ElectionResultsHub> hubContext,
            IResultsAggregator resultsAggregator,
            ILogger<ScheduleTask> logger,
            IElectionConfigurationSource electionConfigurationSource,
            IOptions<AppConfig> config)
            : base(serviceScopeFactory, config, electionConfigurationSource, logger)
        {
            _csvDownloaderJob = csvDownloaderJob;
            _hubContext = hubContext;
            _resultsAggregator = resultsAggregator;
            _logger = logger;
        }

        public override async Task ProcessInScope(IServiceProvider serviceProvider)
        {
            Console.WriteLine($"Processing starts here at {DateTime.UtcNow:F}");
            await _csvDownloaderJob.DownloadFiles();
            _logger.LogInformation("Sending live results");
            var resultsUpdate = SendUpdatedResults();
            var turnoutUpdate = SendUpdatedVoterTurnout();
            var voteMonitoringUpdates = SendUpdatedVoteMonitoringStats();
            await Task.WhenAll(resultsUpdate, turnoutUpdate, voteMonitoringUpdates);
        }

        private async Task SendUpdatedResults()
        {
            _logger.LogInformation($"Sending updated results");
            var provisionalResults = await _resultsAggregator.GetResults(ResultsType.Provisional);
            await _hubContext.Clients.All.SendAsync("results-updated", provisionalResults);
        }

        private async Task SendUpdatedVoterTurnout()
        {
            _logger.LogInformation($"Sending voter turnout updates");
            var voterTurnoutResult = await _resultsAggregator.GetVoterTurnout();
            if (voterTurnoutResult.IsSuccess)
                await _hubContext.Clients.All.SendAsync("turnout-updated", voterTurnoutResult.Value);
        }

        private async Task SendUpdatedVoteMonitoringStats()
        {
            _logger.LogInformation($"Sending vote monitoring updates");
            var stats = await _resultsAggregator.GetVoteMonitoringStats();
            if (stats.IsSuccess)
                await _hubContext.Clients.All.SendAsync("monitoring-updated", stats.Value);
        }
    }
}