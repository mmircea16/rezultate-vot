using System.Linq;
using System.Threading.Tasks;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services;
using ElectionResults.Core.Services.CsvProcessing;
using Microsoft.AspNetCore.Mvc;

namespace ElectionResults.WebApi.Controllers
{
    [Route("api/results")]
    public class ResultsController : Controller
    {
        private readonly IResultsAggregator _resultsAggregator;

        public ResultsController(IResultsAggregator resultsAggregator)
        {
            _resultsAggregator = resultsAggregator;
        }

        [HttpGet("")]
        public async Task<LiveResultsResponse> GetLatestResults([FromQuery] ResultsType type, string location)
        {
            var electionResultsData = await _resultsAggregator.GetResults(type, location);
            var liveResultsResponse = new LiveResultsResponse();

            electionResultsData.Candidates = StatisticsAggregator.CalculatePercentagesForCandidates(electionResultsData.Candidates,
                electionResultsData.Candidates.Sum(c => c.Votes));
            liveResultsResponse.Candidates = electionResultsData.Candidates.Select(c => new CandidateModel
            {
                Id = c.Id,
                ImageUrl = c.ImageUrl,
                Name = c.Name,
                Percentage = c.Percentage,
                Votes = c.Votes
            }).ToList();
            liveResultsResponse.Counties =
                electionResultsData.Candidates.FirstOrDefault()?.Counties.Select(c => new County
                {
                    Label = c.Key,
                    Id = c.Key
                }).ToList();
            liveResultsResponse.Presence = await _resultsAggregator.GetLatestPresence();
            return liveResultsResponse;
        }
    }
}
