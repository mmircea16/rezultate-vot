using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services.CsvProcessing;
using ElectionResults.Core.Storage;
using Newtonsoft.Json;

namespace ElectionResults.Core.Services
{
    public class ResultsAggregator : IResultsAggregator
    {
        private readonly IResultsRepository _resultsRepository;

        public ResultsAggregator(IResultsRepository resultsRepository)
        {
            _resultsRepository = resultsRepository;
        }

        public async Task<LiveResultsResponse> GetResults(ResultsType type, string location = null)
        {
            var liveResultsResponse = new LiveResultsResponse();
            liveResultsResponse.Presence = await GetLatestPresence();
            liveResultsResponse.VoteMonitoringStats = await GetVoteMonitoringStats();

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

        private async Task<VotesPresence> GetLatestPresence()
        {
            var result = await _resultsRepository.GetLatestResults(ResultsLocation.All.ConvertEnumToString(), ResultsType.Presence.ConvertEnumToString());
            return JsonConvert.DeserializeObject<VotesPresence>(result.StatisticsJson);
        }

        private async Task<VoteMonitoringStats> GetVoteMonitoringStats()
        {
            var result = await _resultsRepository.GetLatestResults(ResultsLocation.All.ConvertEnumToString(), ResultsType.VoteMonitoring.ConvertEnumToString());
            return JsonConvert.DeserializeObject<VoteMonitoringStats>(result.StatisticsJson);
        }
    }
}
