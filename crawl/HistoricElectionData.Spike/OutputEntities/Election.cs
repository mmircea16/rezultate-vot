using System;
using HistoricElectionData.Spike.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HistoricElectionData.Spike.OutputEntities
{
    public class Election
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Category Category { get; set; }

        public bool Partial { get; set; }

        public bool Uninominal { get; set; }
    }
}