using Newtonsoft.Json;

namespace ElectionResults.Core.Models
{
    public class CountyInfo
    {
        [JsonProperty("id_county")]
        public string IdCounty { get; set; }
        [JsonProperty("county_code")]
        public string CountyCode { get; set; }
        [JsonProperty("county_name")]
        public string CountyName { get; set; }
        [JsonProperty("initial_count")]
        public int InitialCount { get; set; }
        [JsonProperty("precincts_count")]
        public int PrecinctsCount { get; set; }
        [JsonProperty("LP")]
        public int VotersOnPermanentLists { get; set; }
        [JsonProperty("LS")]
        public int VotersOnSpecialLists { get; set; }
        [JsonProperty("UM")]
        public int MobileVotes { get; set; }
    }
}