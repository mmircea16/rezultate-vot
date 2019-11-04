namespace ElectionResults.Core.Models
{
    public class VotesPresence
    {
        public int EnlistedVoters { get; set; }

        public int TotalNationalVotes { get; set; }

        public decimal PresencePercentage { get; set; }

        public int TotalDiasporaVotes { get; set; }

        public long Timestamp { get; set; }

        public int AdditionalLists { get; set; }

        public int PermanentLists { get; set; }
    }
}