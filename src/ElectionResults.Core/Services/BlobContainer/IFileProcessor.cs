using System.IO;
using System.Threading.Tasks;
using ElectionResults.Core.Models;

namespace ElectionResults.Core.Services.BlobContainer
{
    public interface IFileProcessor
    {
        Task ProcessStream(Stream stream, ElectionResultsFile file);
    }
}