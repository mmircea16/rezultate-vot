using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.S3;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Models;
using ElectionResults.Core.Repositories;
using ElectionResults.Core.Services.BlobContainer;
using ElectionResults.Core.Services.CsvDownload;
using ElectionResults.Core.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace ElectionResults.Tests.CsvDownloaderJobTests
{
    public class DownloadFilesToBlobStorageShould
    {
        private IBucketUploader _bucketUploader;
        private IElectionConfigurationSource _electionConfigurationSource;

        [Fact]
        public async Task RetrieveListOfCsvFiles()
        {
            var csvDownloaderJob = CreatecsvDownloaderJob();
            CreateResultsSourceMock(new ElectionResultsFile());

            await csvDownloaderJob.DownloadFiles();

            _electionConfigurationSource.Received(1).GetListOfFilesWithElectionResults();
        }

        [Fact]
        public async Task UploadFilesToBlobContainer()
        {
            var csvDownloaderJob = CreatecsvDownloaderJob();
            CreateResultsSourceMock(new ElectionResultsFile { Active = true });

            await csvDownloaderJob.DownloadFiles();

            await _bucketUploader
                .Received(1)
                .UploadFromUrl(Arg.Any<ElectionResultsFile>());
        }

        [Fact]
        public async Task UploadSameNumberOfFilesThatItReceived()
        {
            var csvDownloaderJob = CreatecsvDownloaderJob();
            CreateResultsSourceMock(new ElectionResultsFile { Active = true }, new ElectionResultsFile { Active = true });

            await csvDownloaderJob.DownloadFiles();

            await _bucketUploader
                .Received(2)
                .UploadFromUrl(Arg.Any<ElectionResultsFile>());
        }

        [Fact]
        public async Task UseSameTimestampForEachFile()
        {
            var csvDownloaderJob = CreatecsvDownloaderJob();
            CreateResultsSourceMock(new ElectionResultsFile { Active = true }, new ElectionResultsFile { Active = true });
            SystemTime.Now = DateTimeOffset.UtcNow;
            var timestamp = SystemTime.Now.ToUnixTimeSeconds();

            await csvDownloaderJob.DownloadFiles();

            await _bucketUploader
                .Received(2)
                .UploadFromUrl(Arg.Is<ElectionResultsFile>(f => f.Name.Contains(timestamp.ToString())));
        }

        [Fact]
        public async Task BuildNameOfUploadedFiles()
        {
            var csvDownloaderJob = CreatecsvDownloaderJob();
            CreateResultsSourceMock(new ElectionResultsFile { ResultsType = ResultsType.Final, ResultsLocation = ResultsLocation.Romania, Active = true });
            SystemTime.Now = DateTimeOffset.UtcNow;
            var timestamp = SystemTime.Now.ToUnixTimeSeconds();

            await csvDownloaderJob.DownloadFiles();

            await _bucketUploader
                .Received(1)
                .UploadFromUrl(Arg.Is<ElectionResultsFile>(f => f.Name == $"FINAL_RO_{timestamp}.csv"));
        }

        private CsvDownloaderJob CreatecsvDownloaderJob()
        {
            _bucketUploader = Substitute.For<IBucketUploader>();
            _electionConfigurationSource = Substitute.For<IElectionConfigurationSource>();
            var appConfig = new AppConfig { BucketName = "test", TableName = "test" };
            var fakeConfig = new OptionsWrapper<AppConfig>(appConfig);
            var csvDownloaderJob = new CsvDownloaderJob(_bucketUploader, _electionConfigurationSource, new FakeResultsRepository(fakeConfig), new FakeBucketRepository(), fakeConfig);
            return csvDownloaderJob;
        }

        private void CreateResultsSourceMock(params ElectionResultsFile[] files)
        {
            _electionConfigurationSource.GetListOfFilesWithElectionResults()
                .ReturnsForAnyArgs(info => files.ToList());
        }
    }

    internal class FakeBucketRepository : BucketRepository
    {
        public FakeBucketRepository() : base(null)
        {

        }

        public FakeBucketRepository(IAmazonS3 s3Client) : base(s3Client)
        {
        }

        public override Task<bool> DoesS3BucketExist(string bucketName)
        {
            return Task.FromResult(true);
        }
    }

    internal class FakeResultsRepository : ResultsRepository
    {
        public FakeResultsRepository(IOptions<AppConfig> config) : base(config, null, null)
        {

        }

        public FakeResultsRepository(IOptions<AppConfig> config, IAmazonDynamoDB dynamoDb, ILogger<ResultsRepository> logger) : base(config, dynamoDb, logger)
        {
        }

        public override Task InitializeDb()
        {
            return Task.CompletedTask;
        }
    }
}
