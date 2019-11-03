using System.Threading.Tasks;

namespace ElectionResults.Core.Services.CsvDownload
{
    public interface ICsvDownloaderJob
    {
        Task DownloadFiles();
    }
}