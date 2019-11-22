namespace ElectionResults.Core.Models
{
    public class ElectionStatistics
    {
        public string Id { get; set; }

        public string StatisticsJson { get; set; }

        public string Source { get; set; }

        public long Timestamp { get; set; }

        public string Type { get; set; }

        public static ElectionStatistics Default { get; } = new ElectionStatistics();

        public string ElectionId { get; set; }
    }
}
