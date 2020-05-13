using System.Collections.Generic;
using System.Linq;
using HistoricElectionData.Spike.Entities.Abstract;
using HistoricElectionData.Spike.Entities.Summaries;

namespace HistoricElectionData.Spike.Entities.Results
{
    public class Round : Result<ElectionSummary>
    {
        public Dictionary<int, County> Counties { get; set; } = new Dictionary<int, County>();

        public override bool IsEmpty()
        {
            return base.IsEmpty()
                   && (Counties == null || Counties.Count == 0 || Counties.All(county => county.Value.IsEmpty()));
        }
    }
}