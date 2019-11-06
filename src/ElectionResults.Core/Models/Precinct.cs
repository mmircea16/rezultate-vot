using Newtonsoft.Json;

namespace ElectionResults.Core.Models
{
    public class Precinct
    {
        [JsonProperty("LS")]
        public int VotersOnSpecialLists { get; set; }
    }
}