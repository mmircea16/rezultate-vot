using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;
using Newtonsoft.Json;

namespace ElectionResults.Core.Services
{
    public interface IElectionPresenceAggregator
    {
        Task<Result<VotesPresence>> GetCurrentPresence();

        Task<Result<VoteMonitoringStats>> GetVoteMonitoringStats();
    }
}