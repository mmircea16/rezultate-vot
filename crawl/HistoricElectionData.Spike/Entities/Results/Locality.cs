using System.Collections.Generic;
using System.Linq;
using HistoricElectionData.Spike.Entities.Abstract;
using HistoricElectionData.Spike.Entities.Summaries;

namespace HistoricElectionData.Spike.Entities.Results
{
    public class Locality : Result<LocalitySummary>
    {
        public new string Id { get; set; }

        public Dictionary<int, PollingSection> PollingSections { get; set; } = new Dictionary<int, PollingSection>();

        public override bool IsEmpty()
        {
            return base.IsEmpty()
                   && (PollingSections == null || PollingSections.Count == 0 || PollingSections.All(pollingSection => pollingSection.Value.IsEmpty()));
        }
    }
}