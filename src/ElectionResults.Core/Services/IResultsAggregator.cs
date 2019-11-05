using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;

namespace ElectionResults.Core.Services
{
    public interface IResultsAggregator
    {
        Task<LiveResultsResponse> GetResults(ResultsType type, string location = null);
        
        Task<Result<VoteMonitoringStats>> GetVoteMonitoringStats();

        Task<Result<VoterTurnout>> GetVoterTurnout();
    }
}