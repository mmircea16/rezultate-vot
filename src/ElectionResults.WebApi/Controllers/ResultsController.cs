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
            var liveResultsResponse = await _resultsAggregator.GetResults(type, location);
            return liveResultsResponse;
        }
    }
}
