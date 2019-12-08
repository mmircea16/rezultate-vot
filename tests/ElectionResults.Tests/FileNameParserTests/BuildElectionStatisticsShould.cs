using System;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services;
using FluentAssertions;
using Xunit;

namespace ElectionResults.Tests.FileNameParserTests
{
    public class BuildElectionStatisticsShould
    {
        [Fact]
        public void return_a_file_parsing_result()
        {
            var electionStatistics = FileNameParser.BuildElectionStatistics(new ElectionResultsFile
            {
                FileType = FileType.Results,
                ResultsSource = "DSPR",
                Timestamp = 1561818562
            }, new ElectionResultsData());

            electionStatistics.Should().NotBeNull();
        }

        [Fact]
        public void set_properties_from_file()
        {
            var electionStatistics = FileNameParser.BuildElectionStatistics(new ElectionResultsFile{
                FileType = FileType.Results,
                ResultsSource = "DSPR",
                Timestamp = 1561818562
            }, new ElectionResultsData());

            electionStatistics.Source.Should().Be("DSPR");
            electionStatistics.Id.Should().StartWith($"{DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks:D19}".Substring(0, 10));
            electionStatistics.Timestamp.Should().Be(1561818562);
            electionStatistics.StatisticsJson.Should().NotBeNullOrEmpty();
            electionStatistics.Type.Should().Be(FileType.Results.ConvertEnumToString());
        }
    }
}
