using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services.CsvProcessing;
using ElectionResults.Core.Storage;
using Newtonsoft.Json;

namespace ElectionResults.Core.Services
{
    public class ResultsAggregator : IResultsAggregator
    {
        private readonly IResultsRepository _resultsRepository;
        private readonly IElectionConfigurationSource _electionConfigurationSource;

        public ResultsAggregator(IResultsRepository resultsRepository, IElectionConfigurationSource electionConfigurationSource)
        {
            _resultsRepository = resultsRepository;
            _electionConfigurationSource = electionConfigurationSource;
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
