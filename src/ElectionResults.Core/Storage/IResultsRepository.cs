using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;

namespace ElectionResults.Core.Storage
{
    public interface IResultsRepository
    {
        Task InsertResults(ElectionStatistics electionStatistics);

        Task InitializeDb();
        
        Task InsertCurrentVoterTurnout(VoterTurnout voterTurnout);

        Task InsertVoteMonitoringStats(VoteMonitoringStats voteMonitoringInfo);
        
        Task<Result<ElectionStatistics>> Get(string electionId, string source, string type);
    }
}
