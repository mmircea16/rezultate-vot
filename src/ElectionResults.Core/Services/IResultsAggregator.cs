using System.Threading.Tasks;
using ElectionResults.Core.Models;

namespace ElectionResults.Core.Services
{
    public interface IResultsAggregator
    {
        Task<ElectionResultsData> GetResults(ResultsType type, string location = null);
    }
}