using System;
using Newtonsoft.Json;

namespace HistoricElectionData.Spike.JsonSerializationEntities
{
    public class ElectionInfo
    {
        [JsonProperty("Data")]
        public DateTime Date { get; set; }

        [JsonProperty("EsteUninominal")]
        public bool Uninominal { get; set; }
    }
}