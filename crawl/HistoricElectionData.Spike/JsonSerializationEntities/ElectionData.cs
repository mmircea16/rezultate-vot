using Newtonsoft.Json;

namespace HistoricElectionData.Spike.JsonSerializationEntities
{
    public class ElectionData
    {
        [JsonProperty("alegeriId")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Name { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("isPartial")]
        public bool Partial { get; set; }
    }
}