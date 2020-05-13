using Newtonsoft.Json;

namespace HistoricElectionData.Spike.JsonSerializationEntities
{
    public class ReferendumData
    {
        [JsonProperty("VoturiValabileDA")]
        public int VotesYes { get; set; }

        [JsonProperty("ProcentVoturiDA")]
        public double PercentYes { get; set; }

        [JsonProperty("VoturiValabileNU")]
        public int VotesNo { get; set; }

        [JsonProperty("ProcentVoturiNU")]
        public double PercentNo { get; set; }

        [JsonProperty("CodIntrebare")]
        public string Topic { get; set; }

        [JsonProperty("NumarTur", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Round { get; set; }
    }
}