namespace HistoricElectionData.Spike.OutputEntities
{
    public class Summary
    {
        public int IdElection { get; set; }

        public string Topic { get; set; }

        public int Round { get; set; }

        public int IdDivision { get; set; }

        public int Mandates { get; set; }

        public int Enrolled { get; set; }

        public int Voting { get; set; }

        public int Votes { get; set; }

        public int Nulls { get; set; }

        public double PresencePercent { get; set; }

        public int PollingSections { get; set; }

        public string PollingSectionsNumbering { get; set; }

        public int Coefficient { get; set; }

        public int Threshold { get; set; }

        public int Circumscription { get; set; }

        public int MinVotes { get; set; }

        public string Address { get; set; }
    }
}