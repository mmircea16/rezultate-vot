using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services;
using ElectionResults.Core.Services.CsvDownload;
using ElectionResults.Core.Storage;
using ElectionResults.WebApi.Hubs;
using LazyCache;
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
        private readonly IAppCache _appCache;

        public ScheduleTask(IServiceScopeFactory serviceScopeFactory,
            ICsvDownloaderJob csvDownloaderJob,
            IHubContext<ElectionResultsHub> hubContext,
            IResultsAggregator resultsAggregator,
            ILogger<ScheduleTask> logger,
            IElectionConfigurationSource electionConfigurationSource,
            IAppCache appCache,
            IOptions<AppConfig> config)
            : base(serviceScopeFactory, config, electionConfigurationSource, logger)
        {
            _csvDownloaderJob = csvDownloaderJob;
            _hubContext = hubContext;
            _resultsAggregator = resultsAggregator;
            _logger = logger;
            _appCache = appCache;
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
            var key = $"results-{ResultsType.Provisional.ConvertEnumToString()}";
            _appCache.Remove(key);
            var result = await _appCache.GetOrAddAsync(
                key, () => _resultsAggregator.GetResults(ResultsType.Provisional), DateTimeOffset.Now.AddMinutes(1));
            await _hubContext.Clients.All.SendAsync("results-updated", result.Value);
        }

        private async Task SendUpdatedVoterTurnout()
        {
            _appCache.Remove(Consts.VOTE_TURNOUT_KEY);
            var voterTurnoutResult = await _appCache.GetOrAddAsync(Consts.VOTE_TURNOUT_KEY, () => _resultsAggregator.GetVoterTurnout(), DateTimeOffset.Now.AddMinutes(1));
            if (voterTurnoutResult.IsSuccess)
                await _hubContext.Clients.All.SendAsync("turnout-updated", voterTurnoutResult.Value);
        }

        private async Task SendUpdatedVoteMonitoringStats()
        {
            _appCache.Remove(Consts.VOTE_MONITORING_KEY);
            var stats = await _appCache.GetOrAddAsync(Consts.VOTE_MONITORING_KEY, () => _resultsAggregator.GetVoteMonitoringStats(), DateTimeOffset.Now.AddMinutes(1));
            if (stats.IsSuccess)
                await _hubContext.Clients.All.SendAsync("monitoring-updated", stats.Value);
        }
    }
}