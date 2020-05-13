using Newtonsoft.Json;

namespace HistoricElectionData.Spike.JsonSerializationEntities.Lists
{
    public class PollingSectionData
    {
        [JsonProperty("NSV")]
        public int Id { get; set; }

        [JsonProperty("ADRESA")]
        public string Name { get; set; }
    }
}