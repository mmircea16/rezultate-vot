using System;
using ElectionResults.Core.Models;
using Newtonsoft.Json;

namespace ElectionResults.Core.Services
{
    public class FileNameParser
    {
        public static ElectionStatistics BuildElectionStatistics(ElectionResultsFile file, ElectionResultsData electionResultsData)
        {
            var electionStatistics = new ElectionStatistics();
            electionStatistics.Id = $"{DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks:D19}";
            electionStatistics.Type = file.FileType.ConvertEnumToString();
            electionStatistics.Source = file.ResultsSource;
            electionStatistics.Timestamp = file.Timestamp;
            electionStatistics.StatisticsJson = JsonConvert.SerializeObject(electionResultsData);
            electionStatistics.ElectionId = file.ElectionId;
            return electionStatistics;
        }
    }
}
