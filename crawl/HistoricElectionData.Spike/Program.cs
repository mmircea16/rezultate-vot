using System;
using HistoricElectionData.Spike.DataServices;
using Microsoft.Extensions.Configuration;

namespace HistoricElectionData.Spike
{
    public class Program
    {
        public static void Main()
        {
            var outputFolder = GetOutputFolder();

            Fetcher.FetchData(outputFolder);

            Cleaner.CleanupData(outputFolder);

            Denormalizer.DenormalizeData(outputFolder);

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static string GetOutputFolder()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            var configuration = configurationBuilder.Build();

            return configuration["OutputFolder"];
        }
    }
}