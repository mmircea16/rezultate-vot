using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using ElectionResults.Core.Models;
using ElectionResults.Core.Repositories;
using ElectionResults.Core.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElectionResults.Core.Services.BlobContainer
{
    public class BucketUploader : IBucketUploader
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileProcessor _fileProcessor;
        private readonly ILogger<BucketUploader> _logger;
        private static HttpClient _httpClient;
        private readonly AppConfig _config;

        public BucketUploader(IOptions<AppConfig> config, IFileRepository fileRepository, IFileProcessor fileProcessor, ILogger<BucketUploader> logger)
        {
            _fileRepository = fileRepository;
            _fileProcessor = fileProcessor;
            _logger = logger;
            _httpClient = new HttpClient();
            _config = config.Value;
        }

        public async Task UploadFromUrl(ElectionResultsFile file)
        {
            var stream = await DownloadFile(file.URL);
            await UploadFileToStorage(stream, file.Name);
        }

        private async Task<Stream> DownloadFile(string url)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(url);
                return new MemoryStream(Encoding.UTF8.GetBytes(response));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to download file: {url}");
                throw;
            }
        }

        private async Task UploadFileToStorage(Stream fileStream, string fileName)
        {
            try
            {
                var fileData = new FileData { FileName = fileName, Stream = new MemoryStream() };
                fileStream.CopyTo(fileData.Stream);
                var uploadResponse = await _fileRepository.UploadFiles(_config.BucketName, fileData);
                if (uploadResponse.IsSuccess)
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                    await _fileProcessor.ProcessStream(fileStream, fileName);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to process stream for filename: {fileName}");
                throw;
            }
        }
    }
}