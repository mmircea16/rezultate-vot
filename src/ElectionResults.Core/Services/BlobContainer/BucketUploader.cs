using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
            _httpClient = new HttpClient();
            _config = config.Value;
        }

        public async Task UploadFromUrl(ElectionResultsFile file)
        {
            var stream = await DownloadFile(file.URL);
            await UploadFileToStorage(stream, file.Name);
        }

        private static async Task<Stream> DownloadFile(string url)
        {
            var response = await _httpClient.GetStringAsync(url);
            return new MemoryStream(Encoding.UTF8.GetBytes(response));
        }

        private async Task UploadFileToStorage(Stream fileStream, string fileName)
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
    }
}