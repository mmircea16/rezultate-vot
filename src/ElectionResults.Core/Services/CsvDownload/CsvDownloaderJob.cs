using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Models;
using ElectionResults.Core.Repositories;
using ElectionResults.Core.Services.BlobContainer;
using ElectionResults.Core.Storage;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CsvDownloaderJob> _logger;
        private readonly AppConfig _config;

        public CsvDownloaderJob(IBucketUploader bucketUploader,
            IElectionConfigurationSource electionConfigurationSource,
            IResultsRepository resultsRepository,
            IBucketRepository bucketRepository,
            IVoterTurnoutAggregator voterTurnoutAggregator,
            ILogger<CsvDownloaderJob> logger,
            IOptions<AppConfig> config)
        {
            _bucketUploader = bucketUploader;
            _electionConfigurationSource = electionConfigurationSource;
            _resultsRepository = resultsRepository;
            _bucketRepository = bucketRepository;
            _voterTurnoutAggregator = voterTurnoutAggregator;
            _logger = logger;
            _config = config.Value;
            _logger.LogInformation($"Interval is set to: {_config.IntervalInSeconds} seconds");
        }

        public async Task DownloadFiles()
        {
            try
            {
                _logger.LogInformation("Starting to download csv files");
                await InitializeBucket();
                await InitializeDb();
                var electionsConfigJson = await _electionConfigurationSource.GetConfigAsync();
                var elections = JsonConvert.DeserializeObject<List<Election>>(electionsConfigJson.Value);
                foreach (var election in elections)
                {
                    var files = election.Files;

                    var timestamp = SystemTime.Now.ToUnixTimeSeconds();
                    await DownloadCsvFiles(files, timestamp);
                    _logger.LogInformation($"Files downloaded");
                    await AddVoterTurnout(files, timestamp);
                    _logger.LogInformation("Added voter turnout");
                    await AddVoteMonitoringStats(files, timestamp);
                    _logger.LogInformation("Added vote monitoring stats");
                }

                Console.WriteLine($"Finished downloading files");
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "Failed to download files");
            }
        }

        private async Task DownloadCsvFiles(List<ElectionResultsFile> files, long timestamp)
        {
            try
            {
                List<Task> tasks = new List<Task>();
                
                foreach (var file in files.Where(f => f.Active && f.FileType == FileType.Results))
                {
                    _logger.LogInformation($"Downloading file {file.URL}");
                    file.Name =
                        $"{file.FileType.ConvertEnumToString()}_{file.ResultsSource}_{timestamp}.csv";
                    file.Timestamp = timestamp;
                    tasks.Add(_bucketUploader.ProcessFile(file));
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to download csv files");
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
                _logger.LogInformation($"Initializing bucket {bucketName}");
                var bucketExists = await _bucketRepository.DoesS3BucketExist(bucketName);
                if (bucketExists == false)
                {
                    _logger.LogInformation($"Bucket {bucketName} doesn't exist");
                    var response = await _bucketRepository.CreateBucket(bucketName);
                    if (response.IsFailure)
                    {
                        _logger.LogError($"Failed to create bucket: {response.Error}");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to create bucket {_config.BucketName}");
            }
        }
    }
}
