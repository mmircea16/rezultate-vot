using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Infrastructure;
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
        private readonly IElectionConfigurationSource _electionConfigurationSource;
        private decimal _totalCounted;

        public ResultsAggregator(IResultsRepository resultsRepository, ILogger<ResultsAggregator> logger, IElectionConfigurationSource electionConfigurationSource)
        {
            _resultsRepository = resultsRepository;
            _logger = logger;
            _electionConfigurationSource = electionConfigurationSource;
        }

        public async Task<Result<LiveResultsResponse>> GetResults(FileType type, string location = null, string electionId = null)
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
                var voterTurnout = await GetVoterTurnout(electionId);
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

        private async Task<Result<ElectionResultsData>> GetResultsByType(FileType type, string location)
        {
            try
            {
                string resultsType = type.ConvertEnumToString();
                var localResultsResponse = await _resultsRepository.Get(Consts.LOCAL, resultsType);
                var diasporaResultsResponse = await _resultsRepository.Get(Consts.DIASPORA, resultsType);
                if (localResultsResponse.IsFailure || diasporaResultsResponse.IsFailure)
                {
                    return Result.Failure<ElectionResultsData>("Failed to retrieve data");
                }
                var localResultsData = JsonConvert.DeserializeObject<ElectionResultsData>(localResultsResponse.Value.StatisticsJson);
                var diasporaResultsData = JsonConvert.DeserializeObject<ElectionResultsData>(diasporaResultsResponse.Value.StatisticsJson);
                var electionResultsData = StatisticsAggregator.CombineResults(localResultsData, diasporaResultsData);

                electionResultsData.Candidates.FirstOrDefault(c => c.Id == "g1").Votes = 3485292;
                electionResultsData.Candidates.FirstOrDefault(c => c.Id == "g2").Votes = 527098;
                electionResultsData.Candidates.FirstOrDefault(c => c.Id == "g3").Votes = 1384450;
                electionResultsData.Candidates.FirstOrDefault(c => c.Id == "g4").Votes = 357014;
                electionResultsData.Candidates.FirstOrDefault(c => c.Id == "g5").Votes = 2051725;
                electionResultsData.Candidates.FirstOrDefault(c => c.Id == "g6").Votes = 32787;
                electionResultsData.Candidates.FirstOrDefault(c => c.Id == "g7").Votes = 30884;
                electionResultsData.Candidates.FirstOrDefault(c => c.Id == "g8").Votes = 30850;
                electionResultsData.Candidates.FirstOrDefault(c => c.Id == "g9").Votes = 27769;
                electionResultsData.Candidates.FirstOrDefault(c => c.Id == "g10").Votes = 815201;
                electionResultsData.Candidates.FirstOrDefault(c => c.Id == "g11").Votes = 39192;
                electionResultsData.Candidates.FirstOrDefault(c => c.Id == "g12").Votes = 244275;
                electionResultsData.Candidates.FirstOrDefault(c => c.Id == "g13").Votes = 48662;
                electionResultsData.Candidates.FirstOrDefault(c => c.Id == "g14").Votes = 141316;
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
            if (electionResultsData == null)
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

        public async Task<Result<VoteMonitoringStats>> GetVoteMonitoringStats(string electionId)
        {
            var result = await _resultsRepository.Get(electionId, Consts.VOTE_MONITORING_KEY, FileType.VoteMonitoring.ConvertEnumToString());
            if (result.IsSuccess)
            {
                var voteMonitoringStats = JsonConvert.DeserializeObject<VoteMonitoringStats>(result.Value.StatisticsJson);
                return Result.Ok(voteMonitoringStats);
            }
            return Result.Failure<VoteMonitoringStats>("Failed to retrieve vote monitoring stats");
        }

        public async Task<Result<VoterTurnout>> GetVoterTurnout(string electionId)
        {
            var result = await _resultsRepository.Get(electionId, Consts.VOTE_TURNOUT_KEY, FileType.VoterTurnout.ConvertEnumToString());
            if (result.IsSuccess)
            {
                var voterTurnout = JsonConvert.DeserializeObject<VoterTurnout>(result.Value.StatisticsJson);
                return Result.Ok(voterTurnout);
            }

            return Result.Failure<VoterTurnout>("Failed to retrieve voter turnout");
        }

        public async Task<Result<LiveResultsResponse>> GetElectionResults(ResultsQuery resultsQuery)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(resultsQuery.Source))
                {
                    var data = await CombineAllSources(resultsQuery);
                    return Result.Ok(CreateLiveResultsResponse(data));
                }

                var response = await _resultsRepository.Get(resultsQuery.ElectionId, resultsQuery.Source, FileType.Results.ConvertEnumToString());
                if (response.IsSuccess)
                {
                    var electionResultsData = JsonConvert.DeserializeObject<ElectionResultsData>(response.Value.StatisticsJson);
                    if (string.IsNullOrWhiteSpace(resultsQuery.County))
                        return Result.Ok(CreateLiveResultsResponse(electionResultsData));
                    foreach (var candidate in electionResultsData.Candidates)
                    {
                        candidate.Votes = candidate.Counties[resultsQuery.County];
                    }
                    return Result.Ok(CreateLiveResultsResponse(electionResultsData));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return Result.Ok(new LiveResultsResponse());
        }

        private async Task<ElectionResultsData> CombineAllSources(ResultsQuery resultsQuery)
        {
            var files = _electionConfigurationSource.GetListOfFilesWithElectionResults();
            var availableSources = files.Where(f => f.Active && f.FileType == FileType.Results).Select(f => f.Name);
            var data = new ElectionResultsData();
            foreach (var source in availableSources)
            {
                var resultsResponse =
                    await _resultsRepository.Get(resultsQuery.ElectionId, source, FileType.Results.ConvertEnumToString());
                if (resultsResponse.IsSuccess)
                {
                    var deserializedData =
                        JsonConvert.DeserializeObject<ElectionResultsData>(resultsResponse.Value.StatisticsJson);
                    data = StatisticsAggregator.CombineResults(data, deserializedData);
                }
            }
            return data;
        }

        private LiveResultsResponse CreateLiveResultsResponse(ElectionResultsData electionResultsData)
        {
            var candidates = ConvertCandidates(electionResultsData);
            var counties =
                electionResultsData?.Candidates?.FirstOrDefault()?.Counties.Select(c => new County
                {
                    Label = c.Key,
                    Id = "national",
                    CountyName = c.Key
                }).ToList();
            var liveResultsResponse = new LiveResultsResponse
            {
                Candidates = candidates
            };
            liveResultsResponse.Counties = counties ?? new List<County>();
            return liveResultsResponse;
        }
    }
}
