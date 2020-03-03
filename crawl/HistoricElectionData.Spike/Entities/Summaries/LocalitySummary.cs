using HistoricElectionData.Spike.Entities.Abstract;

namespace HistoricElectionData.Spike.Entities.Summaries
{
    public class LocalitySummary : Summary
    {
        public int PollingSections { get; set; }

        public string PollingSectionsNumbering { get; set; }

        public int Coefficient { get; set; }

        public int Threshold { get; set; }

        public int Circumscription { get; set; }

        public int MinVotes { get; set; }

        public override bool IsEmpty()
        {
            return base.IsEmpty()
                   && PollingSections == 0
                   && string.IsNullOrEmpty(PollingSectionsNumbering)
                   && Coefficient == 0
                   && Threshold == 0
                   && Circumscription == 0
                   && MinVotes == 0;
        }
    }
}