using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElectionResults.WebApi.Controllers
{
    [Route("api/settings")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IElectionConfigurationSource _electionConfigurationSource;

        public AdminController(IElectionConfigurationSource electionConfigurationSource)
        {
            _electionConfigurationSource = electionConfigurationSource;
        }

        [HttpPut("interval")]
        public async Task<ActionResult> SetInterval([FromBody] int interval)
        {
            var result = await _electionConfigurationSource.UpdateInterval(interval);
            if (result.IsSuccess)
                return Ok();
            return BadRequest(result.Error);
        }

        [HttpGet("election-config")]
        public async Task<ActionResult> GetSettings()
        {
            var result = await _electionConfigurationSource.GetConfigAsync();
            if (result.IsSuccess)
                return Ok(result.Value);
            return BadRequest(result.Error);
        }

        [HttpPut("election-config")]
        public async Task<ActionResult> UpdateSettings([FromBody] ElectionsConfig config)
        {
            await _electionConfigurationSource.UpdateElectionConfig(config);
            return Ok();
        }
    }
}