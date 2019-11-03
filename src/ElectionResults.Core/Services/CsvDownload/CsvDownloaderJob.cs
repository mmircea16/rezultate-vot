using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Models;
using ElectionResults.Core.Repositories;
using ElectionResults.Core.Services.BlobContainer;
using ElectionResults.Core.Storage;
using Microsoft.Extensions.Options;

namespace ElectionResults.Core.Services.CsvDownload
{
    public class CsvDownloaderJob : ICsvDownloaderJob
    {
        private readonly IBucketUploader _bucketUploader;
        private readonly IElectionConfigurationSource _electionConfigurationSource;
        private readonly IResultsRepository _resultsRepository;
        private readonly IBucketRepository _bucketRepository;
        private readonly AppConfig _config;

        public CsvDownloaderJob(IBucketUploader bucketUploader,
            IElectionConfigurationSource electionConfigurationSource,
            IResultsRepository resultsRepository,
            IBucketRepository bucketRepository,
            IOptions<AppConfig> config)
        {
            _bucketUploader = bucketUploader;
            _electionConfigurationSource = electionConfigurationSource;
            _resultsRepository = resultsRepository;
            _bucketRepository = bucketRepository;
            _config = config.Value;
        }

        public async Task DownloadFiles()
        {
            var files = _electionConfigurationSource.GetListOfFilesWithElectionResults();
            var timestamp = SystemTime.Now.ToUnixTimeSeconds();
            List<Task> tasks = new List<Task>();
            await InitializeBucket();
            await InitializeDb();
            foreach (var file in files.Where(f => f.Active))
            {
                Console.WriteLine($"Downloading file {file}");
                tasks.Add(ProcessCsv(file, timestamp));
            }

            await Task.WhenAll(tasks);
            Console.WriteLine($"Files downloaded");
        }

        private async Task InitializeDb()
        {
            await _resultsRepository.InitializeDb();
        }

        private async Task InitializeBucket()
        {
            var bucketName = _config.BucketName;
            var bucketExists = await _bucketRepository.DoesS3BucketExist(bucketName);
            if (bucketExists == false)
            {
                var response = await _bucketRepository.CreateBucket(bucketName);
                if (response.IsFailure)
                {
                    Console.WriteLine(response.Error);
                    return;
                }
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
