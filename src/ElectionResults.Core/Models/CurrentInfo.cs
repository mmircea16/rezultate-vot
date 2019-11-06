using Newtonsoft.Json;

namespace ElectionResults.Core.Models
{
    public class CurrentInfo
    {
        public string Hour { get; set; }

        [JsonProperty("hour_name")]
        public string HourName { get; set; }

        [JsonProperty("county_id")]
        public string CountyId { get; set; }

        [JsonProperty("county_name")]
        public string CountyName { get; set; }

        [JsonProperty("county_code")]
        public string CountyCode { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("csv")]
        public string Csv { get; set; }
    }
}