using System;
using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services;
using ElectionResults.Core.Services.CsvDownload;
using ElectionResults.Core.Storage;
using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ElectionResults.WebApi.Controllers
{
    [Route("api/results")]
    public class ResultsController : Controller
    {
        private readonly IResultsAggregator _resultsAggregator;
        private readonly IOptionsSnapshot<AppConfig> _config;
        private readonly IAppCache _appCache;

        public ResultsController(IResultsAggregator resultsAggregator,
            IOptionsSnapshot<AppConfig> config,
            IAppCache appCache)
        {
            _resultsAggregator = resultsAggregator;
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
                    Log.LogWarning(result.Error);
                    return BadRequest(result.Error);
                }
                var resultsCount = _appCache.Get<VoteCountStats>(Consts.RESULTS_COUNT_KEY + electionId);
                if (resultsCount != null)
                {
                    result.Value.TotalCountedVotes = resultsCount.TotalCountedVotes;
                    result.Value.PercentageCounted = resultsCount.Percentage;
                }
                return result.Value;
            }
            catch (Exception e)
            {
                Log.LogError(e, "Exception encountered while retrieving results");
                throw;
            }
        }

        [HttpGet("voter-turnout")]
        public async Task<ActionResult<VoterTurnout>> GetVoterTurnout([FromQuery] string electionId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(electionId))
                    electionId = Consts.FirstElectionRound;
                var key = Consts.VOTE_TURNOUT_KEY + electionId;
                var result = await _appCache.GetOrAddAsync(
                    key, () => _resultsAggregator.GetVoterTurnout(electionId),
                    DateTimeOffset.Now.AddSeconds(_config.Value.TurnoutCacheIntervalInSeconds));
                if (result.IsFailure)
                {
                    _appCache.Remove(key);
                    Log.LogWarning(result.Error);
                    return BadRequest(result.Error);
                }
                return result.Value;
            }
            catch (Exception e)
            {
                Log.LogError(e, "Exception encountered while retrieving voter turnout stats");
                return StatusCode(500, e);
            }
        }

        [HttpGet("monitoring")]
        public async Task<ActionResult<VoteMonitoringStats>> GetVoteMonitoringStats([FromQuery] string electionId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(electionId))
                    electionId = Consts.FirstElectionRound;
                var key = Consts.VOTE_MONITORING_KEY + electionId;
                var result = await _appCache.GetOrAddAsync(
                    key, () => _resultsAggregator.GetVoteMonitoringStats(electionId),
                    DateTimeOffset.Now.AddMinutes(5));
                if (result.IsFailure)
                {
                    _appCache.Remove(key);
                    Log.LogWarning(result.Error);
                    return BadRequest(result.Error);
                }
                return result.Value;
            }
            catch (Exception e)
            {
                Log.LogError(e, "Exception encountered while retrieving vote monitoring stats");
                return StatusCode(500, e);
            }
        }
    }
}
