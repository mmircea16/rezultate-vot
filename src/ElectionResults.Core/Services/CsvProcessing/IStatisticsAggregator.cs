using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;

namespace ElectionResults.Core.Services.CsvProcessing
{
    public interface IStatisticsAggregator
    {
        Task<Result<ElectionResultsData>> RetrieveElectionData(string csvContent, ElectionResultsFile file);

        List<ICsvParser> CsvParsers { get; set; }
    }
}
