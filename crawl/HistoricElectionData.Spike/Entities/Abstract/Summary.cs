namespace HistoricElectionData.Spike.Entities.Abstract
{
    public abstract class Summary
    {
        public int Mandates { get; set; }

        public int Enrolled { get; set; }

        public int Voting { get; set; }

        public int Votes { get; set; } 

        public int Nulls { get; set; }

        public double PresencePercent { get; set; }

        public virtual bool IsEmpty()
        {
            return Mandates == 0
                   && Enrolled == 0
                   && Voting == 0
                   && Votes == 0
                   && Nulls == 0;
        }
    }
}