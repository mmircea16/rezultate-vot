using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;

namespace ElectionResults.Core.Services.CsvProcessing
{
    public interface ICsvParser
    {
        Task<Result<ElectionResultsData>> Parse(ElectionResultsData electionResultsData, string csvContent,
            ElectionResultsFile file);
    }
}