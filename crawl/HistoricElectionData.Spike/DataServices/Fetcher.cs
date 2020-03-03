using System;
using System.Collections.Generic;
using System.IO;
using HistoricElectionData.Spike.DataServices.Fetchers;
using HistoricElectionData.Spike.DataServices.Fetchers.Abstract;
using HistoricElectionData.Spike.Entities;
using Newtonsoft.Json;

namespace HistoricElectionData.Spike.DataServices
{
    public class Fetcher
    {
        public static void FetchData(string outputFolder)
        {
            var elections = GetElections(outputFolder);

            var result = JsonConvert.SerializeObject(
                elections,
                Formatting.Indented
            );

            File.WriteAllText($"{outputFolder}all_elections.json", result);
        }

        private static IEnumerable<Election> GetElections(string outputFolder)
        {
            var elections = new List<Election>();

            var electionsData = ElectionFetcher.GetElectionData();

            foreach (var electionData in electionsData)
            {
                var election = new Election
                {
                    Id = electionData.Id,
                    Name = electionData.Name,
                    Category = (Category)Enum.Parse(typeof(Category), electionData.Category, true),
                    Partial = electionData.Partial
                };

                var fetcher = GetElectionFetcherFor(election);

                fetcher.FetchElectionData();

                elections.Add(election);

                File.WriteAllText(
                    $"{outputFolder}election_{election.Id}_{DateTime.Now.ToFileTime()}.json",
                    JsonConvert.SerializeObject(
                        election,
                        Formatting.Indented
                    )
                );
            }

            return elections;
        }

        private static ElectionFetcher GetElectionFetcherFor(Election election)
        {
            ElectionFetcher fetcher;

            switch (election.Category)
            {
                case Category.EuroParlamentare:
                    fetcher = new EuroParlamentareFetcher(election);
                    break;
                case Category.Locale:
                    fetcher = new LocaleFetcher(election);
                    break;
                case Category.Parlamentare:
                    // they fucked up their numbering, so...
                    if ((election.Id == 248) && election.Name.Contains("2016"))
                    {
                        fetcher = new FuckedUpParlamentareFetcher(election);
                    }
                    else
                    {
                        fetcher = new ParlamentareFetcher(election);
                    }
                    break;
                case Category.Prezidentiale:
                    fetcher = new PrezidentialeFetcher(election);
                    break;
                case Category.Referendum:
                    fetcher = new ReferendumFetcher(election);
                    break;
                default:
                    throw new Exception("Category not handled");
            }

            return fetcher;
        }
    }
}