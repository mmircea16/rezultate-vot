using Newtonsoft.Json;

namespace HistoricElectionData.Spike.JsonSerializationEntities.Summaries
{
    public class ElectionSummary
    {
        [JsonProperty("NumarSectii", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int PollingSections { get; set; }

        [JsonProperty("TotalInscrisi", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Enrolled { get; set; }

        [JsonProperty("TotalPrezenti", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Voting { get; set; }

        [JsonProperty("TotalVoturiValabile", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Votes { get; set; }

        [JsonProperty("TotalVoturiNule", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Nulls { get; set; }

        [JsonProperty("ProcentPrezenta", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public double PresencePercent { get; set; }

        [JsonProperty("NumarTur", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Round { get; set; }

        [JsonProperty("CodIntrebare", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public string Topic { get; set; }
    }
}