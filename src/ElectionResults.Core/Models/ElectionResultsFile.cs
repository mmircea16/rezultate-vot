namespace ElectionResults.Core.Models
{
    public class ElectionResultsFile
    {
        public string URL { get; set; }

        public bool Active { get; set; }

        public string Name { get; set; }

        public string ElectionId { get; set; }

        public FileType FileType { get; set; }

        public string ResultsSource { get; set; }

        public long Timestamp { get; set; }

        public string Prefix { get; set; }
    }
}