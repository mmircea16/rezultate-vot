using System.Collections.Generic;
using HistoricElectionData.Spike.Entities.Results;

namespace HistoricElectionData.Spike.Entities
{
    public class Topic
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Dictionary<int, Round> Rounds = new Dictionary<int, Round>();
    }
}