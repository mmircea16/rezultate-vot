
using Newtonsoft.Json;

namespace HistoricElectionData.Spike.JsonSerializationEntities.Summaries
{
    public class CountySummary : ElectionSummary
    {
        [JsonProperty("NumerotareSectii")]
        public string PollingSectionsNumbering { get; set; }

        [JsonProperty("NumarMandate", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Mandates { get; set; }

        [JsonProperty("CoeficientElectoral", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Coefficient { get; set; }

        [JsonProperty("PragElectoral", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Threshold { get; set; }

        [JsonProperty("NrCircumscriptieLoc", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Circumscription { get; set; }

        [JsonProperty("NumarMinimVoturi", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int MinVotes { get; set; }
    }
}