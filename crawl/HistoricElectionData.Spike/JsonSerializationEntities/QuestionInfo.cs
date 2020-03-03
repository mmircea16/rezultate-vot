using Newtonsoft.Json;

namespace HistoricElectionData.Spike.JsonSerializationEntities
{
    public class QuestionInfo
    {
        [JsonProperty("Cod")]
        public int Id { get; set; }

        [JsonProperty("Intrebare")]
        public string Name { get; set; }
    }
}