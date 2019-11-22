using System;
using System.Threading.Tasks;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services;
using ElectionResults.Core.Storage;
using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElectionResults.WebApi.Controllers
{
    [Route("api/results")]
    public class ResultsController : Controller
    {
        private readonly IResultsAggregator _resultsAggregator;
        private readonly ILogger<ResultsController> _logger;
        private readonly IOptionsSnapshot<AppConfig> _config;
        private readonly IAppCache _appCache;

        public ResultsController(IResultsAggregator resultsAggregator,
            ILogger<ResultsController> logger,
            IOptionsSnapshot<AppConfig> config,
            IAppCache appCache)
        {
            _resultsAggregator = resultsAggregator;
            _logger = logger;
            _config = config;
            _appCache = appCache;
        }

        [HttpGet("")]
        public async Task<ActionResult<LiveResultsResponse>> GetResults([FromQuery] string electionId,  string source = null, string county = null, FileType fileType = FileType.Results)
        {
            try
            {
                var resultsQuery = new ResultsQuery(fileType, source, county, electionId);
                var key = resultsQuery.ToString();
                var result = await _appCache.GetOrAddAsync(
                    key, () => _resultsAggregator.GetElectionResults(resultsQuery),
                    DateTimeOffset.Now.AddSeconds(_config.Value.IntervalInSeconds));
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
        public async Task<ActionResult<VoterTurnout>> GetVoterTurnout([FromQuery] string electionId)
        {
            try
            {
                var result = await _appCache.GetOrAddAsync(
                    Consts.VOTE_TURNOUT_KEY, () => _resultsAggregator.GetVoterTurnout(electionId),
                    DateTimeOffset.Now.AddSeconds(_config.Value.TurnoutCacheIntervalInSeconds));
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
        public async Task<ActionResult<VoteMonitoringStats>> GetVoteMonitoringStats([FromQuery] string electionId)
        {
            try
            {
                var result = await _appCache.GetOrAddAsync(
                    Consts.VOTE_MONITORING_KEY, () => _resultsAggregator.GetVoteMonitoringStats(electionId),
                    DateTimeOffset.Now.AddMinutes(5));
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
