namespace ElectionResults.Core.Models
{
    public class CandidateModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public int Votes { get; set; }

        public decimal Percentage { get; set; }
    }
}