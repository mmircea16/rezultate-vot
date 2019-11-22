using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;
using Newtonsoft.Json;

namespace ElectionResults.Core.Services
{
    public interface IVoterTurnoutAggregator
    {
        Task<Result<VoterTurnout>> GetVoterTurnoutFromBEC(ElectionResultsFile turnoutJson);

        Task<Result<VoteMonitoringStats>> GetVoteMonitoringStats(ElectionResultsFile monitoringJson);
    }
}