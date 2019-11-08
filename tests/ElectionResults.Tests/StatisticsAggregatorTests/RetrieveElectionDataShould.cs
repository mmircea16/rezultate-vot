using System.Collections.Generic;
using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure.CsvModels;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services.CsvProcessing;
using ElectionResults.Tests.StatisticsAggregatorTests.Fakes;
using FluentAssertions;
using Xunit;

namespace ElectionResults.Tests.StatisticsAggregatorTests
{
    public class RetrieveElectionDataShould
    {
        [Fact]
        public async Task apply_all_defined_aggregations()
        {
            var firstParser = new FakeCandidatesParser();
            var secondParser = new FakeCandidatesParser();
            var csvParsers = new List<ICsvParser>
            {
                firstParser,
                secondParser
            };
            var statisticsAggregator = new StatisticsAggregator(null) { CsvParsers = csvParsers };
            await statisticsAggregator.RetrieveElectionData("");

            firstParser.WasInvoked.Should().BeTrue();
            secondParser.WasInvoked.Should().BeTrue();
        }

        [Fact]
        public async Task build_election_results_model_with_results_from_each_parser()
        {
            var csvParsers = new List<ICsvParser>
            {
                new FakeCandidatesParser(),
                new FakePollingStationsParser()
            };
            var statisticsAggregator = new StatisticsAggregator(null) { CsvParsers = csvParsers };

            var aggregationResult = await statisticsAggregator.RetrieveElectionData("");

            aggregationResult.Value.Candidates.Should().NotBeNull();
        }

        [Fact]
        public void combine_candidate_votes_by_id()
        {
            var localResults = new ElectionResultsData();
            var diasporaResults = new ElectionResultsData();
            localResults.Candidates = new List<CandidateConfig>
            {
                new CandidateConfig{Id = "L1", Votes = 1},
                new CandidateConfig{Id = "L2", Votes = 2},
            };
            diasporaResults.Candidates = new List<CandidateConfig>
            {
                new CandidateConfig{Id = "L2", Votes = 5},
                new CandidateConfig{Id = "L1", Votes = 10},
            };
            var combinedVotes = StatisticsAggregator.CombineResults(localResults, diasporaResults);

            combinedVotes.Candidates[0].Id.Should().Be("L1");
            combinedVotes.Candidates[0].Votes.Should().Be(11);
            combinedVotes.Candidates[1].Id.Should().Be("L2");
            combinedVotes.Candidates[1].Votes.Should().Be(7);
        }
    }
}
