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
            _logger.LogInformation($"JobTimer is set to: {_config.JobTimer}");
        }

        public async Task DownloadFiles()
        {
            _logger.LogInformation("Starting to download csv files");
            var files = _electionConfigurationSource.GetListOfFilesWithElectionResults();
            var timestamp = SystemTime.Now.ToUnixTimeSeconds();
            List<Task> tasks = new List<Task>();
            await InitializeBucket();
            await InitializeDb();
            await DownloadCsvFiles(files, tasks, timestamp);
            _logger.LogInformation($"Files downloaded");
            await AddVoterTurnout(timestamp);
            _logger.LogInformation("Added voter turnout");
            await AddVoteMonitoringStats(timestamp);
            _logger.LogInformation("Added vote monitoring stats");
        }

        private async Task DownloadCsvFiles(List<ElectionResultsFile> files, List<Task> tasks, long timestamp)
        {
            try
            {
                foreach (var file in files.Where(f => f.Active))
                {
                    _logger.LogInformation($"Downloading file {file.URL}");
                    tasks.Add(ProcessCsv(file, timestamp));
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to download csv files");
            }
        }

        private async Task AddVoteMonitoringStats(long timestamp)
        {
            var result = await _voterTurnoutAggregator.GetVoteMonitoringStats();
            if (result.IsSuccess)
            {
                result.Value.Timestamp = timestamp;
                await _resultsRepository.InsertVoteMonitoringStats(result.Value);
            }
        }

        private async Task AddVoterTurnout(long timestamp)
        {
            var result = await _voterTurnoutAggregator.GetVoterTurnoutFromBEC();
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

        private async Task ProcessCsv(ElectionResultsFile file, long timestamp)
        {
            file.Name =
                $"{file.ResultsType.ConvertEnumToString()}_{file.ResultsLocation.ConvertEnumToString()}_{timestamp}.csv";
            await _bucketUploader.UploadFromUrl(file);
        }
    }
}
