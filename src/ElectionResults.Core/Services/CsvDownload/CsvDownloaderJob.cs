using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Models;
using ElectionResults.Core.Repositories;
using ElectionResults.Core.Services.BlobContainer;
using ElectionResults.Core.Storage;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ElectionResults.Core.Services.CsvDownload
{
    public class CsvDownloaderJob : ICsvDownloaderJob
    {
        private readonly IBucketUploader _bucketUploader;
        private readonly IElectionConfigurationSource _electionConfigurationSource;
        private readonly IResultsRepository _resultsRepository;
        private readonly IBucketRepository _bucketRepository;
        private readonly IVoterTurnoutAggregator _voterTurnoutAggregator;
        private readonly IResultsAggregator _resultsAggregator;
        private readonly IAppCache _appCache;
        private readonly AppConfig _config;

        public CsvDownloaderJob(IBucketUploader bucketUploader,
            IElectionConfigurationSource electionConfigurationSource,
            IResultsRepository resultsRepository,
            IBucketRepository bucketRepository,
            IVoterTurnoutAggregator voterTurnoutAggregator,
            IResultsAggregator resultsAggregator,
            IAppCache appCache,
            IOptions<AppConfig> config)
        {
            _bucketUploader = bucketUploader;
            _electionConfigurationSource = electionConfigurationSource;
            _resultsRepository = resultsRepository;
            _bucketRepository = bucketRepository;
            _voterTurnoutAggregator = voterTurnoutAggregator;
            _resultsAggregator = resultsAggregator;
            _appCache = appCache;
            _config = config.Value;
            Log.LogInformation($"Interval is set to: {_config.IntervalInSeconds} seconds");
        }

        public async Task DownloadFiles()
        {
            try
            {
                await InitializeBucket();
                await InitializeDb();
                var electionsConfigJson = await _electionConfigurationSource.GetConfigAsync();
                var elections = JsonConvert.DeserializeObject<List<Election>>(electionsConfigJson.Value);
                foreach (var election in elections)
                {
                    var files = election.Files;
                    var timestamp = SystemTime.Now.ToUnixTimeSeconds();
                    await DownloadCsvFiles(files, timestamp);
                    await AddVoterTurnout(files, timestamp);
                    await AddVoteMonitoringStats(files, timestamp);
                }
            }
            catch (Exception e)
            {
                Log.LogError(e, "Failed to download files");
            }
        }

        private async Task DownloadCsvFiles(List<ElectionResultsFile> files, long timestamp)
        {
            try
            {
                List<Task> tasks = new List<Task>();

                foreach (var file in files.Where(f => f.Active && f.FileType == FileType.Results))
                {
                    file.Name =
                        $"{file.ElectionId}_{file.FileType.ConvertEnumToString()}_{file.ResultsSource}_{timestamp}.csv";
                    file.Timestamp = timestamp;
                    tasks.Add(_bucketUploader.ProcessFile(file));
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                Log.LogError(e, $"Failed to download csv files");
            }
        }

        private async Task AddVoteMonitoringStats(List<ElectionResultsFile> files, long timestamp)
        {
            var monitoringJson = files.FirstOrDefault(f => f.FileType == FileType.VoteMonitoring);
            if (monitoringJson == null)
                return;
            var result = await _voterTurnoutAggregator.GetVoteMonitoringStats(monitoringJson);
            if (result.IsSuccess)
            {
                result.Value.Timestamp = timestamp;
                await _resultsRepository.InsertVoteMonitoringStats(result.Value);
            }
        }

        private async Task AddVoterTurnout(List<ElectionResultsFile> files, long timestamp)
        {
            var turnoutJson = files.FirstOrDefault(f => f.FileType == FileType.VoterTurnout);
            if (turnoutJson == null)
                return;
            var result = await _voterTurnoutAggregator.GetVoterTurnoutFromBEC(turnoutJson);
            if (result.IsSuccess)
            {
                result.Value.Timestamp = timestamp;
                await _resultsRepository.InsertCurrentVoterTurnout(result.Value);
            }
        }

        private async Task InitializeDb()
        {
            await _resultsRepository.InitializeDb();
        }

        private async Task InitializeBucket()
        {
            try
            {
                var bucketName = _config.BucketName;
                Log.LogInformation($"Initializing bucket {bucketName}");
                var bucketExists = await _bucketRepository.DoesS3BucketExist(bucketName);
                if (bucketExists == false)
                {
                    Log.LogInformation($"Bucket {bucketName} doesn't exist");
                    var response = await _bucketRepository.CreateBucket(bucketName);
                    if (response.IsFailure)
                    {
                        Log.LogWarning($"Failed to create bucket: {response.Error}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogError(e, $"Failed to create bucket {_config.BucketName}");
            }
        }
    }

    public class VoteCountStats
    {
        public int TotalCountedVotes { get; set; }

        public double Percentage { get; set; }
    }
}
