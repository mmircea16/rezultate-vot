using System;
using System.Threading.Tasks;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services;
using ElectionResults.Core.Storage;
using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElectionResults.WebApi.Controllers
{
    [Route("api/results")]
    public class ResultsController : Controller
    {
        private readonly IResultsAggregator _resultsAggregator;
        private readonly ILogger<ResultsController> _logger;
        private readonly IAppCache _appCache;

        public ResultsController(IResultsAggregator resultsAggregator,
            ILogger<ResultsController> logger,
            IAppCache appCache)
        {
            _resultsAggregator = resultsAggregator;
            _logger = logger;
            _appCache = appCache;
        }

        [HttpGet("")]
        public async Task<ActionResult<LiveResultsResponse>> GetLatestResults([FromQuery] ResultsType type, string location)
        {
            try
            {
                var key = $"results-{type.ConvertEnumToString()}{location}";
                var result = await _appCache.GetOrAddAsync(
                    key, () => _resultsAggregator.GetResults(type, location), DateTimeOffset.Now.AddMinutes(5));
                if (result.IsFailure)
                {
                    _appCache.Remove(key);
                    _logger.LogError(result.Error);
                    return BadRequest(result.Error);
                }
                return result.Value;
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
                var result = await _appCache.GetOrAddAsync(
                    Consts.VOTE_TURNOUT_KEY, () => _resultsAggregator.GetVoterTurnout(), DateTimeOffset.Now.AddMinutes(5));
                if (result.IsFailure)
                {
                    _appCache.Remove(Consts.VOTE_TURNOUT_KEY);
                    _logger.LogError(result.Error);
                    return BadRequest(result.Error);
                }
                return result.Value;
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
                var result = await _appCache.GetOrAddAsync(
                    Consts.VOTE_MONITORING_KEY, () => _resultsAggregator.GetVoteMonitoringStats(), DateTimeOffset.Now.AddMinutes(5));
                if (result.IsFailure)
                {
                    _appCache.Remove(Consts.VOTE_TURNOUT_KEY);
                    _logger.LogError(result.Error);
                    return BadRequest(result.Error);
                }
                return result.Value;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception encountered while retrieving vote monitoring stats");
                return StatusCode(500, e);
            }
        }
    }
}
