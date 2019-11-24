using ElectionResults.Core.Services;

namespace ElectionResults.Core.Models
{
    public class ResultsQuery
    {
        public ResultsQuery()
        {
            
        }

        public ResultsQuery(FileType fileType, string source, string county, string electionId)
        {
            FileType = fileType;
            Source = source;
            ElectionId = electionId;
            County = county;
        }

        public string County { get; set; }

        public FileType FileType { get; set; }

        public string Source { get; set; }

        public string ElectionId { get; set; }

        public override string ToString()
        {
            return $"results-{FileType.ConvertEnumToString()}-{Source}-{County}-{ElectionId}";
        }
    }
}