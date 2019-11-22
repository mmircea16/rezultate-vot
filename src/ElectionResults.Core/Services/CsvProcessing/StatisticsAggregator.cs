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
        public async Task<Result<ElectionResultsData>> RetrieveElectionData(string csvContent, ElectionResultsFile file)
        {
            _logger.LogInformation($"Retrieving data from csv");
            try
            {
                var electionResults = new ElectionResultsData();
                foreach (var csvParser in CsvParsers)
                {
                    await csvParser.Parse(electionResults, csvContent, file);
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


        public static ElectionResultsData CombineResults(ElectionResultsData firstResultsSet, ElectionResultsData secondResultsSet)
        {
            var candidates = new List<CandidateConfig>();
            if (firstResultsSet?.Candidates == null)
                return secondResultsSet;
            if (secondResultsSet?.Candidates == null)
                return firstResultsSet;
            foreach (var candidate in firstResultsSet.Candidates)
            {
                candidates.Add(new CandidateConfig
                {
                    Id = candidate.Id,
                    CsvId = candidate.CsvId,
                    ImageUrl = candidate.ImageUrl,
                    Name = candidate.Name,
                    Votes = candidate.Votes + secondResultsSet.Candidates.FirstOrDefault(c => c.Id == candidate.Id)?.Votes ?? 0,
                    Percentage = candidate.Percentage,
                    Counties = candidate.Counties
                });
            }

            candidates = CalculatePercentagesForCandidates(candidates, candidates.Sum(c => c.Votes));

            return new ElectionResultsData{Candidates = candidates};
        }

    }
}