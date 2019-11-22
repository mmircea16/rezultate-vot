using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using CsvHelper;
using ElectionResults.Core.Infrastructure.CsvModels;
using ElectionResults.Core.Models;
using ElectionResults.Core.Storage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ElectionResults.Core.Services.CsvProcessing
{
    public class CandidatesResultsParser : ICsvParser
    {
        private readonly IOptions<AppConfig> _config;

        public CandidatesResultsParser(IOptions<AppConfig> config)
        {
            _config = config;
        }

        public async Task<Result<ElectionResultsData>> Parse(ElectionResultsData electionResultsData, string csvContent, ElectionResultsFile file)
        {
            if (electionResultsData == null)
                electionResultsData = new ElectionResultsData();
            var electionsConfig = DeserializeElectionsConfig();

            electionResultsData.Candidates = electionsConfig.Candidates.Select(c => new CandidateConfig
            {
                Id = c.CsvId,
                ImageUrl = c.ImageUrl,
                Name = c.Name
            }).ToList();
            
            await PopulateCandidatesListWithVotes(csvContent, electionResultsData.Candidates, file);
            var sumOfVotes = electionResultsData.Candidates.Sum(c => c.Votes);
            electionResultsData.Candidates = StatisticsAggregator.CalculatePercentagesForCandidates(electionResultsData.Candidates, sumOfVotes);

            return Result.Ok(electionResultsData);
        }

        private Election DeserializeElectionsConfig()
        {
            try
            {
                return JsonConvert.DeserializeObject<Election>(_config.Value.ElectionsConfig);
            }
            catch (Exception)
            {
                return Election.Default;
            }
        }

        protected virtual async Task PopulateCandidatesListWithVotes(string csvContent,
            List<CandidateConfig> candidates, ElectionResultsFile file)
        {
            try
            {
                var csvParser = new CsvParser(new StringReader(csvContent));
                var headers = (await csvParser.ReadAsync()).ToList();
                do
                {
                    var rowValues = await csvParser.ReadAsync();
                    if (rowValues == null)
                        break;
                    foreach (var candidate in candidates)
                    {
                        var votes = int.Parse(rowValues[headers.IndexOf(file.Prefix + candidate.Id)]);
                        candidate.Votes += votes;
                    }
                } while (true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
