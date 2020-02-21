using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Models;
using ElectionResults.Core.Repositories;
using ElectionResults.Core.Storage;
using Microsoft.Extensions.Options;

namespace ElectionResults.Core.Services.BlobContainer
{
    public class BucketUploader : IBucketUploader
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileProcessor _fileProcessor;
        private static HttpClient _httpClient;
        private readonly AppConfig _config;

        public BucketUploader(IOptions<AppConfig> config, IFileRepository fileRepository, IFileProcessor fileProcessor)
        {
            _fileRepository = fileRepository;
            _fileProcessor = fileProcessor;
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true;
            _httpClient = new HttpClient(httpClientHandler);
            _config = config.Value;
        }

        public async Task ProcessFile(ElectionResultsFile file)
        {
            try
            {
                var stream = await DownloadFile(file.URL);
                var uploadResponse = await UploadFileToStorage(stream, file.Name);
                if (uploadResponse.IsSuccess)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    await _fileProcessor.ProcessStream(stream, file);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
                Log.LogError(e, $"Failed to download file: {url}");
                throw;
            }
        }

        private async Task<Result> UploadFileToStorage(Stream fileStream, string fileName)
        {
            try
            {
                var fileData = new FileData { FileName = fileName, Stream = new MemoryStream() };
                fileStream.CopyTo(fileData.Stream);
                return await _fileRepository.UploadFiles(_config.BucketName, fileData);
            }
            catch (Exception e)
            {
                Log.LogError(e, $"Failed to process stream for filename: {fileName}");
                throw;
            }
        }
    }
}