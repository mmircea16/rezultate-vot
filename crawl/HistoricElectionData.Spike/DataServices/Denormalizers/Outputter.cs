using System.Collections.Generic;
using System.IO;
using HistoricElectionData.Spike.OutputEntities;
using Newtonsoft.Json;

namespace HistoricElectionData.Spike.DataServices.Denormalizers
{
    public class Outputter
    {
        private readonly string _outputFolder;

        public Outputter(string outputFolder)
        {
            _outputFolder = outputFolder;
        }

        public void SaveSummariesAndResultsAndFlushData(int idElection, List<Summary> summaries, List<Result> results)
        {
            OutputData($"summaries_{idElection}", summaries);
            OutputData($"results_{idElection}", results);

            summaries.Clear();
            results.Clear();
        }

        public void OutputElections(List<Election> elections)
        {
            OutputData("elections", elections);
        }

        public void OutputDivisions(List<Division> divisions)
        {
            OutputTaxonomies("divisions", divisions);
        }

        private void OutputTaxonomies(string fileName, object taxonomy)
        {
            OutputResults($"{_outputFolder}taxonomy\\{fileName}.json", taxonomy);
        }

        private void OutputData(string fileName, object data)
        {
            OutputResults($"{_outputFolder}data\\{fileName}.json", data);
        }

        private static void OutputResults(string path, object results)
        {
            File.WriteAllText(
                path,
                JsonConvert.SerializeObject(
                    results,
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }
                )
            );
        }
    }
}
