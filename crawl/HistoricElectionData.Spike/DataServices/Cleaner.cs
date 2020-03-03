using System.IO;
using System.Linq;
using HistoricElectionData.Spike.Entities;
using HistoricElectionData.Spike.Entities.Abstract;
using HistoricElectionData.Spike.Entities.Results;
using Newtonsoft.Json;

namespace HistoricElectionData.Spike.DataServices
{
    public static class Cleaner
    {
        public static void CleanupData(string outputFolder)
        {
            var electionFiles = Directory.GetFiles(outputFolder, "election_*.json");

            foreach (var electionFile in electionFiles)
            {
                var election = JsonConvert.DeserializeObject<Election>(File.ReadAllText(electionFile));

                FixEmptyElectionData(election);

                File.WriteAllText(
                    electionFile,
                    JsonConvert.SerializeObject(
                        election,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }
                    )
                );
            }
        }

        private static void FixEmptyElectionData(Election election)
        {
            foreach (var topic in election.Topics.Values)
            {
                foreach (var round in topic.Rounds.Values)
                {
                    foreach (var county in round.Counties.Values)
                    {
                        foreach (var locality in county.Localities.Values)
                        {
                            RemoveEmptyPollingSections(locality);
                        }

                        RemoveEmptyLocalities(county);
                    }

                    RemoveEmptyCounties(round);
                }

                RemoveEmptyRounds(topic);
            }
        }

        private static void MarkEmptyDataAsNull<T>(Result<T> result) where T : Summary, new()
        {
            if (result.Summary.IsEmpty())
            {
                result.Summary = null;
            }

            if (result.Results.Count == 0)
            {
                result.Results = null;
            }
        }

        private static void RemoveEmptyRounds(Topic topic)
        {
            var roundIdsToRemove = topic.Rounds.Where(
                round => round.Value.IsEmpty()
            ).Select(removeEntity => removeEntity.Key).ToList();

            foreach (var roundId in roundIdsToRemove)
            {
                topic.Rounds.Remove(roundId);
            }
        }

        private static void RemoveEmptyCounties(Round round)
        {
            var countyIdsToRemove = round.Counties.Where(
                county => county.Value.IsEmpty()
            ).Select(removeEntity => removeEntity.Key).ToList();

            foreach (var countyId in countyIdsToRemove)
            {
                round.Counties.Remove(countyId);
            }

            MarkEmptyDataAsNull(round);
        }

        private static void RemoveEmptyLocalities(County county)
        {
            var localityIdsToRemove = county.Localities.Where(
                locality => locality.Value.IsEmpty()
            ).Select(removeEntity => removeEntity.Key).ToList();

            foreach (var localityId in localityIdsToRemove)
            {
                county.Localities.Remove(localityId);
            }

            MarkEmptyDataAsNull(county);
        }

        private static void RemoveEmptyPollingSections(Locality locality)
        {
            foreach (var pollingSection in locality.PollingSections.Values)
            {
                MarkEmptyDataAsNull(pollingSection);
            }

            var pollingSectionIdsToRemove = locality.PollingSections.Where(
                pollingSection => pollingSection.Value.IsEmpty()
            ).Select(removeEntity => removeEntity.Key).ToList();

            foreach (var pollingSectionId in pollingSectionIdsToRemove)
            {
                locality.PollingSections.Remove(pollingSectionId);
            }

            MarkEmptyDataAsNull(locality);
        }
    }
}