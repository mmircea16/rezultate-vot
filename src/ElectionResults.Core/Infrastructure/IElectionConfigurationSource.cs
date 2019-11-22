using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;

namespace ElectionResults.Core.Infrastructure
{
    public interface IElectionConfigurationSource
    {
        Task<Result> UpdateInterval(int newTimer);

        Task<Result<int>> GetInterval();

        List<ElectionResultsFile> GetListOfFilesWithElectionResults();

        Task<Result> UpdateElectionConfig(Election config);

        Task<Result<string>> GetConfigAsync();
    }
}
