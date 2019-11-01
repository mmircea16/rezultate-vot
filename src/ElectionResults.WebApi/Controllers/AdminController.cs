using System.Collections.Generic;
using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Infrastructure.CsvModels;
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

        [HttpPut("jobTimer")]
        public async Task<ActionResult> SetJobTimer([FromBody] string interval)
        {
            var result = await _electionConfigurationSource.UpdateJobTimer(interval);
            if (result.IsSuccess)
                return Ok();
            return BadRequest(result.Error);
        }

        [HttpGet("election-config")]
        public ActionResult GetSettings()
        {
            return Ok(_electionConfigurationSource.GetConfig());
        }

        [HttpPut("election-config")]
        public async Task<ActionResult> UpdateSettings([FromBody] ElectionsConfig config)
        {
            //temporary config until the admin frontend is ready
            var newConfig = new ElectionsConfig();
            newConfig.Candidates = new List<CandidateConfig>
            {
                new CandidateConfig
                {
                    Name = "Candidate A",
                    CsvId = "g1",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c1.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate B",
                    CsvId = "g2",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c2.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate C",
                    CsvId = "g3",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c1.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate D",
                    CsvId = "g4",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c2.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate E",
                    CsvId = "g5",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c1.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate F",
                    CsvId = "g6",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c2.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate G",
                    CsvId = "g7",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c1.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate H",
                    CsvId = "g8",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c2.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate I",
                    CsvId = "g9",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c1.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate J",
                    CsvId = "g10",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c2.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate K",
                    CsvId = "g11",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c1.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate L",
                    CsvId = "g12",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c2.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate M",
                    CsvId = "g13",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c1.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate N",
                    CsvId = "g14",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c2.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate O",
                    CsvId = "g15",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c1.png"
                },
                new CandidateConfig
                {
                    Name = "Candidate P",
                    CsvId = "g16",
                    ImageUrl = "https://code4-presidential-2019.s3.eu-central-1.amazonaws.com/c2.png"
                }
            };
            newConfig.Files = new List<ElectionResultsFile>
            {
                new ElectionResultsFile
                {
                    Active = true,
                    ResultsType = ResultsType.Partial,
                    ResultsLocation = ResultsLocation.Romania,
                    URL = "https://prezenta.bec.ro/europarlamentare26052019/data/pv/csv/pv_RO_EUP_PART.csv"
                },
                new ElectionResultsFile
                {
                    Active = true,
                    ResultsType = ResultsType.Partial,
                    ResultsLocation = ResultsLocation.Diaspora,
                    URL = "https://prezenta.bec.ro/europarlamentare26052019/data/pv/csv/pv_SR_EUP_PART.csv"
                },
                new ElectionResultsFile
                {
                    Active = true,
                    ResultsType = ResultsType.Final,
                    ResultsLocation = ResultsLocation.Romania,
                    URL = "https://prezenta.bec.ro/europarlamentare26052019/data/pv/csv/pv_RO_EUP_FINAL.csv"
                },
                new ElectionResultsFile
                {
                    Active = true,
                    ResultsType = ResultsType.Final,
                    ResultsLocation = ResultsLocation.Diaspora,
                    URL = "https://prezenta.bec.ro/europarlamentare26052019/data/pv/csv/pv_SR_EUP_FINAL.csv"
                },
                new ElectionResultsFile
                {
                    Active = true,
                    ResultsType = ResultsType.Provisional,
                    ResultsLocation = ResultsLocation.Romania,
                    URL = "https://prezenta.bec.ro/europarlamentare26052019/data/pv/csv/pv_RO_EUP_PROV.csv"
                },
                new ElectionResultsFile
                {
                    Active = true,
                    ResultsType = ResultsType.Provisional,
                    ResultsLocation = ResultsLocation.Diaspora,
                    URL = "https://prezenta.bec.ro/europarlamentare26052019/data/pv/csv/pv_SR_EUP_PROV.csv"
                }
            };
            await _electionConfigurationSource.UpdateElectionConfig(newConfig);
            return Ok();
        }
    }
}