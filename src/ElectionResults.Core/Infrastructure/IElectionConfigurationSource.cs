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

        Task<Result> UpdateElectionConfig(List<Election> elections);

        Task<Result<string>> GetConfigAsync();
        Result<Election> GetElectionById(string electionId);
    }
}
