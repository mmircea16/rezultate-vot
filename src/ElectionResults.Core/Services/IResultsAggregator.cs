using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;

namespace ElectionResults.Core.Services
{
    public interface IResultsAggregator
    {
        Task<Result<VoteMonitoringStats>> GetVoteMonitoringStats(string electionId);

        Task<Result<VoterTurnout>> GetVoterTurnout(string electionId);
        
        Task<Result<LiveResultsResponse>> GetElectionResults(ResultsQuery resultsQuery);
    }
}