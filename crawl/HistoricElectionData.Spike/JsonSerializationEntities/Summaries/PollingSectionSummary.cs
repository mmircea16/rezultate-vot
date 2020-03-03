
using Newtonsoft.Json;

namespace HistoricElectionData.Spike.JsonSerializationEntities.Summaries
{
    public class PollingSectionSummary : ElectionSummary
    {
        [JsonProperty("Adresa")]
        public string Address { get; set; }
    }
}