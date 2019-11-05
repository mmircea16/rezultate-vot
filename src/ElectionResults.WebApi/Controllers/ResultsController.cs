using System;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElectionResults.WebApi.Controllers
{
    [Route("api/results")]
    public class ResultsController : Controller
    {
        private readonly IResultsAggregator _resultsAggregator;
        private readonly ILogger<ResultsController> _logger;

        public ResultsController(IResultsAggregator resultsAggregator, ILogger<ResultsController> logger)
        {
            _resultsAggregator = resultsAggregator;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<LiveResultsResponse> GetLatestResults([FromQuery] ResultsType type, string location)
        {
            var liveResultsResponse = await _resultsAggregator.GetResults(type, location);
            return liveResultsResponse;
        }

        [HttpGet("voter-turnout")]
        public async Task<ActionResult<VoterTurnout>> GetVoterTurnout()
        {
            try
            {
                _logger.LogInformation("Retrieving voter turnout stats");
                var result = await _resultsAggregator.GetVoterTurnout();
                if (result.IsSuccess)
                    return result.Value;
                return BadRequest(result.Error);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception encountered while retrieving voter turnout stats");
                return StatusCode(500, e);
            }
        }

        [HttpGet("monitoring")]
        public async Task<ActionResult<VoteMonitoringStats>> GetVoteMonitoringStats()
        {
            try
            {
                _logger.LogInformation("Retrieving vote monitoring stats");
                var result = await _resultsAggregator.GetVoteMonitoringStats();
                if (result.IsSuccess)
                    return result.Value;
                return BadRequest(result.Error);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception encountered while retrieving vote monitoring stats");
                return StatusCode(500, e);
            }
        }
    }
}
