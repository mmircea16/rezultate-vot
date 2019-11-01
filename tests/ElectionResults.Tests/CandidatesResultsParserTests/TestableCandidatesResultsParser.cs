using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure.CsvModels;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services.CsvProcessing;
using ElectionResults.Core.Storage;
using Microsoft.Extensions.Options;

namespace ElectionResults.Tests.CandidatesResultsParserTests
{
    public class TestableCandidatesResultsParser: CandidatesResultsParser
    {
        public TestableCandidatesResultsParser(IOptions<AppConfig> config) : base(config)
        {

        }

        protected override Task PopulateCandidatesListWithVotes(string csvContent, List<CandidateConfig> candidates)
        {
            return Task.CompletedTask;
        }

        public List<CandidateConfig> ParsedCandidates { get; set; }
    }
}