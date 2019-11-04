using System.Collections.Generic;
using ElectionResults.Core.Infrastructure.CsvModels;
using ElectionResults.Core.Services;

namespace ElectionResults.Core.Models
{
    public class ElectionResultsData
    {
        public List<CandidateConfig> Candidates { get; set; }

        public VotesPresence Presence { get; set; }
    }
}
