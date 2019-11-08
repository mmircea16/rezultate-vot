namespace ElectionResults.Core.Models
{
    public class VoterTurnout
    {
        public int EnlistedVoters { get; set; }

        public int TotalNationalVotes { get; set; }

        public decimal TurnoutPercentage { get; set; }

        public int TotalDiasporaVotes { get; set; }

        public long Timestamp { get; set; }

        public int AdditionalLists { get; set; }

        public int PermanentLists { get; set; }
        
        public int MobileVotes { get; set; }

        public int DiasporaWithoutMailVotes { get; set; }
    }
}