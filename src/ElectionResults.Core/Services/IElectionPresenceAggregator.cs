using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;

namespace ElectionResults.Core.Services
{
    public interface IElectionPresenceAggregator
    {
        Task<Result<VotesPresence>> GetCurrentPresence();
    }

    public class VotesPresence
    {
        public int EnlistedVoters { get; set; }

        public int TotalNationalVotes { get; set; }

        public decimal PresencePercentage { get; set; }
        
        public int TotalDiasporaVotes { get; set; }
        
        public long Timestamp { get; set; }
    }

    public class VotingPresenceResponse
    {
        [JsonProperty("current_info")]
        public CurrentInfo CurrentInfo { get; set; }

        [JsonProperty("county")]
        public CountyInfo[] Counties { get; set; }

        [JsonProperty("precinct")]
        public Precinct[] Precinct { get; set; }
    }

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

    public class Precinct
    {
        [JsonProperty("LS")]
        public int VotersOnSpecialLists { get; set; }
    }
}