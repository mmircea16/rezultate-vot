using HistoricElectionData.Spike.Entities.Abstract;

namespace HistoricElectionData.Spike.Entities.Summaries
{
    public class SectionSummary : Summary
    {
        public string Address { get; set; }

        public override bool IsEmpty()
        {
            return base.IsEmpty()
                 && string.IsNullOrEmpty(Address);
        }
    }
}