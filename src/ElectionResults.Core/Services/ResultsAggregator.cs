using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Infrastructure.CsvModels;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services.CsvProcessing;
using ElectionResults.Core.Storage;
using LazyCache;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ElectionResults.Core.Services
{
    public class ResultsAggregator : IResultsAggregator
    {
        private readonly IResultsRepository _resultsRepository;
        private readonly ILogger<ResultsAggregator> _logger;
        private decimal _totalCounted;

        public ResultsAggregator(IResultsRepository resultsRepository, ILogger<ResultsAggregator> logger)
        {
            _resultsRepository = resultsRepository;
            _logger = logger;
        }

        public async Task<Result<LiveResultsResponse>> GetResults(ResultsType type, string location = null)
        {
            try
            {
                var liveResultsResponse = new LiveResultsResponse();

                var result = await GetResultsByType(type, location);

                if (result.IsFailure)
                    return Result.Failure<LiveResultsResponse>("Could not load results");
                var selectedResults = result.Value;
                var candidates = ConvertCandidates(selectedResults);
                
                var counties =
                    selectedResults?.Candidates?.FirstOrDefault()?.Counties.Select(c => new County
                    {
                        Label = c.Key,
                        Id = c.Key
                    }).ToList();
                liveResultsResponse.Candidates = candidates;
                liveResultsResponse.Counties = counties ?? new List<County>();
                var voterTurnout = await GetVoterTurnout();
                if (voterTurnout.IsSuccess)
                {
                    decimal totalVotes = voterTurnout.Value.TotalNationalVotes;
                    var percentage = Math.Round(_totalCounted / totalVotes, 2) * 100;
                    liveResultsResponse.PercentageCounted = percentage;
                    liveResultsResponse.VoterTurnout = totalVotes;
                }
                return Result.Ok(liveResultsResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Encountered exception while retrieving results");
                throw;
            }
        }

        private async Task<Result<ElectionResultsData>> GetResultsByType(ResultsType type, string location)
        {
            try
            {
                string resultsType = type.ConvertEnumToString();
                var localResultsResponse = await _resultsRepository.GetLatestResults(Consts.LOCAL, resultsType);
                var diasporaResultsResponse = await _resultsRepository.GetLatestResults(Consts.DIASPORA, resultsType);
                if (localResultsResponse.IsFailure || diasporaResultsResponse.IsFailure)
                {
                    return Result.Failure<ElectionResultsData>("Failed to retrieve data");
                }
                var localResultsData = JsonConvert.DeserializeObject<ElectionResultsData>(localResultsResponse.Value.StatisticsJson);
                var diasporaResultsData = JsonConvert.DeserializeObject<ElectionResultsData>(diasporaResultsResponse.Value.StatisticsJson);
                var electionResultsData = StatisticsAggregator.CombineResults(localResultsData, diasporaResultsData);
                _totalCounted = electionResultsData.Candidates.Sum(c => c.Votes);
                if (string.IsNullOrWhiteSpace(location) == false)
                {
                    if (location == "TOTAL")
                    {
                        return Result.Ok(electionResultsData);
                    }
                    if (location == "DSPR")
                    {
                        return Result.Ok(diasporaResultsData);
                    }
                    if (location == "RO")
                    {
                        return Result.Ok(localResultsData);
                    }
                    foreach (var candidate in electionResultsData.Candidates)
                    {
                        candidate.Votes = candidate.Counties[location];
                    }
                }
                return Result.Ok(electionResultsData);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to retrieve results for type {type} and location {location}");
                return Result.Failure<ElectionResultsData>(e.Message);
            }
        }

        private List<CandidateModel> ConvertCandidates(ElectionResultsData electionResultsData)
        {
            if(electionResultsData == null)
                return new List<CandidateModel>();
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
            if (result.IsSuccess)
            {
                var voteMonitoringStats = JsonConvert.DeserializeObject<VoteMonitoringStats>(result.Value.StatisticsJson);
                return Result.Ok(voteMonitoringStats);
            }
            return Result.Failure<VoteMonitoringStats>("Failed to retrieve vote monitoring stats");
        }

        public async Task<Result<VoterTurnout>> GetVoterTurnout()
        {
            var result = await _resultsRepository.GetLatestResults(ResultsLocation.All.ConvertEnumToString(), ResultsType.VoterTurnout.ConvertEnumToString());
            if (result.IsSuccess)
            {
                var voterTurnout = JsonConvert.DeserializeObject<VoterTurnout>(result.Value.StatisticsJson);
                return Result.Ok(voterTurnout);
            }

            return Result.Failure<VoterTurnout>("Failed to retrieve voter turnout");
        }
    }
}
