using System.Threading.Tasks;
using ElectionResults.Core.Models;

namespace ElectionResults.Core.Services
{
    public interface IResultsAggregator
    {
        Task<LiveResultsResponse> GetResults(ResultsType type, string location = null);
    }
}