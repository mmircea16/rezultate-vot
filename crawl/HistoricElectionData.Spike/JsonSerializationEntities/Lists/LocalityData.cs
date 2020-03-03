using Newtonsoft.Json;

namespace HistoricElectionData.Spike.JsonSerializationEntities.Lists
{
    public class LocalityData
    {
        [JsonProperty("SIRUTA")]
        public string Id { get; set; }

        [JsonProperty("Nume")]
        public string Name { get; set; }
    }
}