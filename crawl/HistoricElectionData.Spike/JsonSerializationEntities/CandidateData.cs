using Newtonsoft.Json;

namespace HistoricElectionData.Spike.JsonSerializationEntities
{
    public class CandidateData
    {
        [JsonProperty("Candidat")]
        public string Name { get; set; }

        [JsonProperty("AfilierePolitica")]
        public string Party { get; set; }

        [JsonProperty("Voturi")]
        public int Votes { get; set; }

        [JsonProperty("Procent")]
        public double Percent { get; set; }

        [JsonProperty("DenumireScurta", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public string ShortName { get; set; }

        [JsonProperty("NumarTur", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Round { get; set; }

        [JsonProperty("NumarMandate1", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Mandates1 { get; set; }

        [JsonProperty("NumarMandate2", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Mandates2 { get; set; }

        [JsonProperty("NumarMandateTotal", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Mandates { get; set; }

        [JsonProperty("PestePragElectoral", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public bool OverElectoralThreshold { get; set; }

        [JsonProperty("NumarCandidati", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public int Candidates { get; set; }
    }
}