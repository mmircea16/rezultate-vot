using System.Collections.Generic;
using ElectionResults.Core.Infrastructure.CsvModels;

namespace ElectionResults.Core.Models
{
    public class ElectionsConfig
    {
        public List<Election> Elections { get; set; }
    }

    public class Election
    {
        public string ElectionId { get; set; }

        public List<ElectionResultsFile> Files { get; set; }

        public List<CandidateConfig> Candidates { get; set; }

        public static Election Default => new Election
        {
            Candidates = new List<CandidateConfig>(),
            Files = new List<ElectionResultsFile>()
        };
    }
}