using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services;

namespace ElectionResults.Core.Storage
{
    public interface IResultsRepository
    {
        Task InsertResults(ElectionStatistics electionStatistics);

        Task<Result<ElectionStatistics>> GetLatestResults(string location, string type);
        
        Task InitializeDb();
        
        Task InsertCurrentVoterTurnout(VoterTurnout voterTurnout);

        Task InsertVoteMonitoringStats(VoteMonitoringStats voteMonitoringInfo);
    }
}
