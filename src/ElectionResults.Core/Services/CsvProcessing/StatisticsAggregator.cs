using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Infrastructure.CsvModels;
using ElectionResults.Core.Models;
using Microsoft.Extensions.Logging;

namespace ElectionResults.Core.Services.CsvProcessing
{
    public class StatisticsAggregator : IStatisticsAggregator
    {
        private readonly ILogger _logger;
        public List<ICsvParser> CsvParsers { get; set;  } = new List<ICsvParser>();

        public StatisticsAggregator(ILogger<StatisticsAggregator> logger)
        {
            _logger = logger;
        }
        public async Task<Result<ElectionResultsData>> RetrieveElectionData(string csvContent)
        {
            _logger.LogInformation($"Retrieving data from csv");
            try
            {
                var electionResults = new ElectionResultsData();
                foreach (var csvParser in CsvParsers)
                {
                    await csvParser.Parse(electionResults, csvContent);
                }

                return Result.Ok(electionResults);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to process data from csv");
                _logger.LogInformation(csvContent.Substring(0, 1000));
                return Result.Failure<ElectionResultsData>(e.Message);
            }
        }


        public static List<CandidateConfig> CalculatePercentagesForCandidates(List<CandidateConfig> candidates, int sumOfVotes)
        {
            try
            {
                foreach (var candidate in candidates)
                {
                    decimal percentage = Math.Round((decimal)candidate.Votes / sumOfVotes * 100, 2);
                    candidate.Percentage = percentage;
                }
                return candidates.OrderByDescending(c => c.Percentage).ToList();
            }
            catch (Exception e)
            {
                return candidates.OrderByDescending(c => c.Percentage).ToList();
            }
        }


        public static ElectionResultsData CombineResults(ElectionResultsData localResults, ElectionResultsData diasporaResults)
        {
            var candidates = new List<CandidateConfig>();
            if (localResults == null)
                return diasporaResults;
            if (diasporaResults == null)
                return localResults;
            for (int i = 0; i < localResults.Candidates.Count; i++)
            {
                var candidate = localResults.Candidates[i];
                candidates.Add(new CandidateConfig
                {
                    Id = candidate.Id,
                    CsvId = candidate.CsvId,
                    ImageUrl = candidate.ImageUrl,
                    Name = candidate.Name,
                    Votes = candidate.Votes + diasporaResults.Candidates.FirstOrDefault(c => c.Id == candidate.Id).Votes,
                    Percentage = candidate.Percentage,
                    Counties = candidate.Counties
                });
            }

            candidates = CalculatePercentagesForCandidates(candidates, candidates.Sum(c => c.Votes));

            return new ElectionResultsData{Candidates = candidates};
        }

    }
}