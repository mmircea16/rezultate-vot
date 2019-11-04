using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services.CsvProcessing;

namespace ElectionResults.Tests.StatisticsAggregatorTests.Fakes
{
    public class FakePollingStationsParser : ICsvParser
    {
        public Task<Result<ElectionResultsData>> Parse(ElectionResultsData electionResultsData, string csvContent)
        {
            return Task.FromResult(Result.Ok(electionResultsData));
        }
    }
}