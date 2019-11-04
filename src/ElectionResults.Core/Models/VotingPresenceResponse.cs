using Newtonsoft.Json;

namespace ElectionResults.Core.Models
{
    public class VotingPresenceResponse
    {
        [JsonProperty("current_info")]
        public CurrentInfo CurrentInfo { get; set; }

        [JsonProperty("county")]
        public CountyInfo[] Counties { get; set; }

        [JsonProperty("precinct")]
        public Precinct[] Precinct { get; set; }
    }
}