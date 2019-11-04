using System.Collections.Generic;
using ElectionResults.Core.Models;

namespace ElectionResults.Core.Infrastructure.CsvModels
{
    public class CandidateConfig: CandidateModel
    {
        public string CsvId { get; set; }

        public Dictionary<string, int> Counties { get; set; } = new Dictionary<string, int>();
    }
}