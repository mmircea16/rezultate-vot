using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ElectionResults.Core.Infrastructure;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services.CsvProcessing;
using ElectionResults.Core.Storage;
using Microsoft.Extensions.Options;

namespace ElectionResults.Core.Services.BlobContainer
{
    public class FileProcessor : IFileProcessor
    {
        private readonly IResultsRepository _resultsRepository;
        private readonly IStatisticsAggregator _statisticsAggregator;

        public FileProcessor(IResultsRepository resultsRepository,
            IStatisticsAggregator statisticsAggregator,
            IOptions<AppConfig> config)
        {
            _resultsRepository = resultsRepository;
            _statisticsAggregator = statisticsAggregator;
            _statisticsAggregator.CsvParsers = new List<ICsvParser>
            {
                new CandidatesResultsParser(config),
                new CountyParser(config)
            };
        }

        public async Task ProcessStream(Stream csvStream, ElectionResultsFile file)
        {
            Log.LogInformation($"Processing csv with stream of {csvStream.Length} bytes");
            var csvContent = await ReadCsvContent(csvStream);
            var aggregationResult = await _statisticsAggregator.RetrieveElectionData(csvContent, file);
            if (aggregationResult.IsSuccess)
            {
                var electionStatistics = FileNameParser.BuildElectionStatistics(file, aggregationResult.Value);
                Log.LogInformation($"Inserting results from {file.Name} with timestamp {electionStatistics.Timestamp}");
                
                await _resultsRepository.InsertResults(electionStatistics);
            }
        }

        protected virtual async Task<string> ReadCsvContent(Stream csvStream)
        {
            var buffer = new byte[csvStream.Length];
            await csvStream.ReadAsync(buffer, 0, (int)csvStream.Length);
            var csvContent = Encoding.UTF8.GetString(buffer);
            return csvContent;
        }
    }
}
