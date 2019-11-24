using System.Collections.Generic;

namespace ElectionResults.Core.Models
{
    public class LiveResultsResponse
    {
        public List<CandidateModel> Candidates { get; set; }

        public List<County> Counties { get; set; }

        public double PercentageCounted { get; set; }

        public decimal TotalCountedVotes { get; set; }
    }
}