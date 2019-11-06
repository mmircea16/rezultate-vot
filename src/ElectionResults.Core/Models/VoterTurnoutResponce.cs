using Newtonsoft.Json;

namespace ElectionResults.Core.Models
{
    public class VoterTurnoutResponce
    {
        [JsonProperty("current_info")]
        public CurrentInfo CurrentInfo { get; set; }

        [JsonProperty("county")]
        public CountyInfo[] Counties { get; set; }

        [JsonProperty("precinct")]
        public Precinct[] Precinct { get; set; }
    }
}