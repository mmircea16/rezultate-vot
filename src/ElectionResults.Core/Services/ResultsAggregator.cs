using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services.CsvProcessing;
using ElectionResults.Core.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ElectionResults.Core.Services
{
    public class ResultsAggregator : IResultsAggregator
    {
        private readonly IResultsRepository _resultsRepository;
        private readonly ILogger<ResultsAggregator> _logger;

        public ResultsAggregator(IResultsRepository resultsRepository, ILogger<ResultsAggregator> logger)
        {
            _resultsRepository = resultsRepository;
            _logger = logger;
        }

        public async Task<LiveResultsResponse> GetResults(ResultsType type, string location = null)
        {
            try
            {
                var liveResultsResponse = new LiveResultsResponse();
                var voterTurnoutResult = await GetVoterTurnout();
                _logger.LogInformation("Retrieved voter turnout");
                if (voterTurnoutResult.IsSuccess)
                    liveResultsResponse.VoterTurnout = voterTurnoutResult.Value;

                var selectedResults = await GetResultsByType(type, location);
                var candidates = ConvertCandidates(selectedResults);
                var counties =
                    selectedResults.Candidates.FirstOrDefault()?.Counties.Select(c => new County
                    {
                        Label = c.Key,
                        Id = c.Key
                    }).ToList();
                liveResultsResponse.Candidates = candidates;
                liveResultsResponse.Counties = counties;

                return liveResultsResponse;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Encountered exception while retrieving results");
                throw;
            }
        }

        private async Task<ElectionResultsData> GetResultsByType(ResultsType type, string location)
        {
            string resultsType = type.ConvertEnumToString();
            var localResults = await _resultsRepository.GetLatestResults(Consts.LOCAL, resultsType);
            var diasporaResults = await _resultsRepository.GetLatestResults(Consts.DIASPORA, resultsType);
            var localResultsData = JsonConvert.DeserializeObject<ElectionResultsData>(localResults.StatisticsJson);
            var diasporaResultsData = JsonConvert.DeserializeObject<ElectionResultsData>(diasporaResults.StatisticsJson);
            var electionResultsData = StatisticsAggregator.CombineResults(localResultsData, diasporaResultsData);
            if (string.IsNullOrWhiteSpace(location) == false)
            {
                if (location == "TOTAL")
                {
                    return electionResultsData;
                }
                if (location == "DSPR")
                {
                    return diasporaResultsData;
                }
                if (location == "RO")
                {
                    return localResultsData;
                }
                foreach (var candidate in electionResultsData.Candidates)
                {
                    candidate.Votes = candidate.Counties[location];
                }
            }

            return electionResultsData;
        }

        private List<CandidateModel> ConvertCandidates(ElectionResultsData electionResultsData)
        {
            electionResultsData.Candidates = StatisticsAggregator.CalculatePercentagesForCandidates(
                electionResultsData.Candidates,
                electionResultsData.Candidates.Sum(c => c.Votes));
            var candidates = electionResultsData.Candidates.Select(c => new CandidateModel
            {
                Id = c.Id,
                ImageUrl = c.ImageUrl,
                Name = c.Name,
                Percentage = c.Percentage,
                Votes = c.Votes
            }).ToList();
            return candidates;
        }

        public async Task<Result<VoteMonitoringStats>> GetVoteMonitoringStats()
        {
            var result = await _resultsRepository.GetLatestResults(ResultsLocation.All.ConvertEnumToString(), ResultsType.VoteMonitoring.ConvertEnumToString());
            var voteMonitoringStats = JsonConvert.DeserializeObject<VoteMonitoringStats>(result.StatisticsJson);
            return Result.Ok(voteMonitoringStats);
        }

        public async Task<Result<VoterTurnout>> GetVoterTurnout()
        {
            var result = await _resultsRepository.GetLatestResults(ResultsLocation.All.ConvertEnumToString(), ResultsType.VoterTurnout.ConvertEnumToString());
            var voterTurnout = JsonConvert.DeserializeObject<VoterTurnout>(result.StatisticsJson);
            return Result.Ok(voterTurnout);
        }
    }
}
