using System.Collections.Generic;
using ElectionResults.Core.Infrastructure.CsvModels;

namespace ElectionResults.Core.Models
{
    public class PollingStation
    {
        public string County { get; set; }

        public string Name { get; set; }

        public List<CandidateConfig> Candidates { get; set; } = new List<CandidateConfig>();

        public int TotalVotes { get; set; }
    }
}