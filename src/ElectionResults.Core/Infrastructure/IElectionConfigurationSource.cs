using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;

namespace ElectionResults.Core.Infrastructure
{
    public interface IElectionConfigurationSource
    {
        Task<Result> UpdateJobTimer(string newTimer);

        Task<Result<string>> GetJobTimer();

        List<ElectionResultsFile> GetListOfFilesWithElectionResults();

        Task<Result> UpdateElectionConfig(ElectionsConfig config);

        Task<Result<string>> GetConfigAsync();
    }
}
