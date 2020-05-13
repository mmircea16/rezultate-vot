namespace HistoricElectionData.Spike.Entities
{
    public class Candidate
    {
        public string Name { get; set; }

        public string Party { get; set; }

        public string ShortName { get; set; }

        public int Votes { get; set; }

        public double Percent { get; set; }

        public int VotesYes { get; set; }

        public double PercentYes { get; set; }

        public int VotesNo { get; set; }

        public double PercentNo { get; set; }

        public int Mandates1 { get; set; }

        public int Mandates2 { get; set; }

        public int Mandates { get; set; }

        public bool OverElectoralThreshold { get; set; }

        public int Candidates { get; set; }
    }
}