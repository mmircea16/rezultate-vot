using System;
using System.Threading.Tasks;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services;
using ElectionResults.WebApi.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ElectionResults.WebApi.Controllers
{
    [Route("api/results")]
    public class ResultsController : Controller
    {
        private readonly IResultsAggregator _resultsAggregator;
        private readonly ILogger<ResultsController> _logger;
        private readonly IHubContext<ElectionResultsHub> _hubContext;

        public ResultsController(IResultsAggregator resultsAggregator,
            ILogger<ResultsController> logger,
            IHubContext<ElectionResultsHub> hubContext)
        {
            _resultsAggregator = resultsAggregator;
            _logger = logger;
            _hubContext = hubContext;
        }

        [HttpGet("")]
        public async Task<LiveResultsResponse> GetLatestResults([FromQuery] ResultsType type, string location)
        {
            try
            {
                _logger.LogInformation("Retrieving vote results");
                var liveResultsResponse = await _resultsAggregator.GetResults(type, location);
                await _hubContext.Clients.All.SendAsync("results-updated", liveResultsResponse);
                return liveResultsResponse;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception encountered while retrieving results");
                throw;
            }
        }

        [HttpGet("voter-turnout")]
        public async Task<ActionResult<VoterTurnout>> GetVoterTurnout()
        {
            try
            {
                _logger.LogInformation("Retrieving voter turnout stats");
                var result = await _resultsAggregator.GetVoterTurnout();
                if (result.IsSuccess)
                {
                    await _hubContext.Clients.All.SendAsync("turnout-updated", result.Value);
                    return result.Value;
                }
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
                {
                    await _hubContext.Clients.All.SendAsync("monitoring-updated", result.Value);
                    return result.Value;
                }
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
