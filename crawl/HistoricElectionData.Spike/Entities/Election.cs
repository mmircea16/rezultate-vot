using System;
using System.Collections.Generic;

namespace HistoricElectionData.Spike.Entities
{
    public class Election
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public Category Category { get; set; }

        public bool Partial { get; set; }

        public bool Uninominal { get; set; }

        public Dictionary<string, Topic> Topics = new Dictionary<string, Topic>();
    }
}