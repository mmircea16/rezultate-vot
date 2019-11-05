using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Models;
using Newtonsoft.Json;

namespace ElectionResults.Core.Services
{
    public class VoterTurnoutAggregator : IVoterTurnoutAggregator
    {
        private readonly IElectionConfigurationSource _electionConfigurationSource;

        public VoterTurnoutAggregator(IElectionConfigurationSource electionConfigurationSource)
        {
            _electionConfigurationSource = electionConfigurationSource;
        }

        public virtual async Task<Result<VoterTurnout>> GetVoterTurnoutFromBEC()
        {
            try
            {
                var httpClient = new HttpClient();
                var files = _electionConfigurationSource.GetListOfFilesWithElectionResults();
                var turnoutJson = files.FirstOrDefault(f => f.ResultsType == ResultsType.VoterTurnout);
                if (turnoutJson == null)
                    return Result.Failure<VoterTurnout>("File not available");
                var json = await httpClient.GetStringAsync(turnoutJson?.URL);
                var voterTurnoutResponse = JsonConvert.DeserializeObject<VoterTurnoutResponce>(json);
                var permanentLists = voterTurnoutResponse.Counties.Sum(c => c.VotersOnPermanentLists);
                var specialLists = voterTurnoutResponse.Counties.Sum(c => c.VotersOnSpecialLists);
                var mobileVotes = voterTurnoutResponse.Counties.Sum(c => c.MobileVotes);

                var diasporaVoters = voterTurnoutResponse.Precinct.Sum(c => c.VotersOnSpecialLists);
                var enlistedVoters = voterTurnoutResponse.Counties.Sum(c => c.InitialCount);
                var totalVoters = permanentLists + mobileVotes + (specialLists - diasporaVoters);
                var voterTurnoutPercentage = totalVoters / (decimal)enlistedVoters;
                var voterTurnout = new VoterTurnout
                {
                    EnlistedVoters = enlistedVoters,
                    TurnoutPercentage = Math.Round(voterTurnoutPercentage * 100, 2),
                    TotalNationalVotes = totalVoters,
                    TotalDiasporaVotes = diasporaVoters,
                    PermanentLists = permanentLists,
                    AdditionalLists = specialLists - diasporaVoters
                };
                return Result.Ok(voterTurnout);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Result.Failure<VoterTurnout>(e.Message);
            }
        }

        public async Task<Result<VoteMonitoringStats>> GetVoteMonitoringStats()
        {
            var httpClient = new HttpClient();
            var files = _electionConfigurationSource.GetListOfFilesWithElectionResults();
            var voteMonitoringJson = files.FirstOrDefault(f => f.ResultsType == ResultsType.VoteMonitoring);
            var json = await httpClient.GetStringAsync(voteMonitoringJson.URL);
            var response = JsonConvert.DeserializeObject<List<MonitoringInfo>>(json);
            return Result.Ok(new VoteMonitoringStats
            {
                Statistics = response
            });
        }
    }
}