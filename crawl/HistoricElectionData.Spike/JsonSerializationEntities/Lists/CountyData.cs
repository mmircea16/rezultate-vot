using Newtonsoft.Json;

namespace HistoricElectionData.Spike.JsonSerializationEntities.Lists
{
    public class CountyData
    {
        [JsonProperty("COD_JUD")]
        public int Id { get; set; }

        [JsonProperty("DEN_JUD")]
        public string Name { get; set; }
    }
}