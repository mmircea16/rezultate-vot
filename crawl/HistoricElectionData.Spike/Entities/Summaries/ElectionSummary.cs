using HistoricElectionData.Spike.Entities.Abstract;

namespace HistoricElectionData.Spike.Entities.Summaries
{
    public class ElectionSummary : Summary
    {
        public int PollingSections { get; set; }

        public override bool IsEmpty()
        {
            return base.IsEmpty()
                   && PollingSections == 0;
        }
    }
}