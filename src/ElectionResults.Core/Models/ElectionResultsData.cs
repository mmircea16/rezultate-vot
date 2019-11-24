using System.Collections.Generic;
using ElectionResults.Core.Infrastructure.CsvModels;

namespace ElectionResults.Core.Models
{
    public class ElectionResultsData
    {
        public List<CandidateConfig> Candidates { get; set; }

        public static ElectionResultsData Default => new ElectionResultsData { Candidates = new List<CandidateConfig>() };
    }
}
