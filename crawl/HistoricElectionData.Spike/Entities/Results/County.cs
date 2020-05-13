using System.Collections.Generic;
using System.Linq;
using HistoricElectionData.Spike.Entities.Abstract;
using HistoricElectionData.Spike.Entities.Summaries;

namespace HistoricElectionData.Spike.Entities.Results
{
    public class County : Result<LocalitySummary>
    {
        public Dictionary<string, Locality> Localities { get; set; } = new Dictionary<string, Locality>();

        public override bool IsEmpty()
        {
            return base.IsEmpty()
                   && (Localities == null || Localities.Count == 0 || Localities.All(locality => locality.Value.IsEmpty()));
        }
    }
}