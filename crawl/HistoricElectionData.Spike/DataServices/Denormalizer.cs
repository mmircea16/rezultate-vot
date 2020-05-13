using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HistoricElectionData.Spike.DataServices.Denormalizers;
using HistoricElectionData.Spike.Entities;
using HistoricElectionData.Spike.Entities.Results;
using HistoricElectionData.Spike.Entities.Summaries;
using HistoricElectionData.Spike.OutputEntities;
using Newtonsoft.Json;
using Election = HistoricElectionData.Spike.OutputEntities.Election;

namespace HistoricElectionData.Spike.DataServices
{
    public class Denormalizer
    {
        private readonly List<Election> _elections = new List<Election>();
        private readonly List<Division> _divisions = new List<Division>();
        private readonly List<Result> _results = new List<Result>();
        private readonly List<Summary> _summaries = new List<Summary>();
        private readonly Outputter _outputter;

        private readonly string _outputFolder;
        private int _lastUniqueDivisionId;

        public static void DenormalizeData(string outputFolder)
        {
            var denormalizer = new Denormalizer(outputFolder);

            denormalizer.Denormalize();
        }

        private Denormalizer(string outputFolder)
        {
            _outputFolder = outputFolder;
            _outputter = new Outputter(_outputFolder);
        }

        private void Denormalize()
        {
            var electionFiles = Directory.GetFiles(_outputFolder, "election_*.json");

            var idElection = 0;
            foreach (var electionFile in electionFiles)
            {
                Console.Write($"Started {idElection + 1}...");
                var election = JsonConvert.DeserializeObject<Entities.Election>(File.ReadAllText(electionFile));

                _elections.Add(
                    new Election
                    {
                        Id = ++idElection,
                        Name = election.Name.ToFormWithoutDiacritics(),
                        Date = election.Date,
                        Category = election.Category,
                        Partial = election.Partial,
                        Uninominal = election.Uninominal
                    }
                );

                DenormalizeTopics(idElection, election);

                _outputter.SaveSummariesAndResultsAndFlushData(idElection, _summaries, _results);

                Console.WriteLine(" done.");
            }

            _outputter.OutputElections(_elections);
            _outputter.OutputDivisions(_divisions);
        }

        private void DenormalizeTopics(int idElection, Entities.Election election)
        {
            foreach (var topic in election.Topics.Values)
            {
                var topicName = GetTopicNameFrom(topic, election);

                DenormalizeRounds(idElection, topicName, topic.Rounds);
            }
        }

        private void DenormalizeRounds(int idElection, string topicName, Dictionary<int, Round> topicRounds)
        {
            var roundsCount = topicRounds.Count;

            foreach (var round in topicRounds.Values)
            {
                var roundName = GetRoundNameFrom(round.Id, roundsCount);

                DenormalizeRound(idElection, topicName, roundName, round);
            }
        }

        private void DenormalizeRound(int idElection, string topicName, int roundName, Round round)
        {
            AddSummaryIfNotNull(idElection, topicName, roundName, 0, round.Summary);
            
            AddResults(round.Results, idElection, topicName, roundName, 0);

            foreach (var county in round.Counties.Values)
            {
                DenormalizeCounty(idElection, topicName, roundName, county);
            }
        }

        private void DenormalizeCounty(int idElection, string topicName, int roundName, County county)
        {
            var idCounty = GetDivisionFrom(county.Name, 0).Id;

            AddSummaryIfNotNull(idElection, topicName, roundName, idCounty, county.Summary);

            AddResults(county.Results, idElection, topicName, roundName, idCounty);

            foreach (var locality in county.Localities.Values)
            {
                DenormalizeLocality(idElection, topicName, roundName, idCounty, locality);
            }
        }

        private void DenormalizeLocality(int idElection, string topicName, int roundName, int idCounty, Locality locality)
        {
            var idLocality = GetDivisionFrom(locality.Name, idCounty).Id;

            AddSummaryIfNotNull(idElection, topicName, roundName, idLocality, locality.Summary);

            AddResults(locality.Results, idElection, topicName, roundName, idLocality);

            foreach (var pollingSection in locality.PollingSections.Values)
            {
                DenormalizePollingSection(idElection, topicName, roundName, idLocality, pollingSection);
            }
        }

        private void DenormalizePollingSection(int idElection, string topicName, int roundName, int idLocality, PollingSection pollingSection)
        {
            var idPollingSection = GetDivisionFrom(pollingSection.Name, idLocality).Id;

            AddSummaryIfNotNull(idElection, topicName, roundName, idLocality, pollingSection.Summary);

            if (pollingSection.Summary != null && !pollingSection.Summary.IsEmpty())
            {
                _summaries.Add(
                    new Summary
                    {
                        IdElection = idElection,
                        Topic = topicName,
                        Round = roundName,
                        IdDivision = idPollingSection,
                        Mandates = pollingSection.Summary.Mandates,
                        Enrolled = pollingSection.Summary.Enrolled,
                        Voting = pollingSection.Summary.Voting,
                        Votes = pollingSection.Summary.Votes,
                        Nulls = pollingSection.Summary.Nulls,
                        PresencePercent = pollingSection.Summary.PresencePercent,
                        Address = pollingSection.Summary.Address.NormalizeName()
                    }
                );
            }

            AddResults(pollingSection.Results, idElection, topicName, roundName, idPollingSection);
        }

        private void AddSummaryIfNotNull<T>(int idElection, string topicName, int roundName, int idDivision, T inputSummary) where T : Entities.Abstract.Summary
        {
            if (inputSummary == null || inputSummary.IsEmpty())
            {
                return;
            }

            var summary = new Summary
            {
                IdElection = idElection,
                Topic = topicName,
                Round = roundName,
                IdDivision = idDivision,
                Mandates = inputSummary.Mandates,
                Enrolled = inputSummary.Enrolled,
                Voting = inputSummary.Voting,
                Votes = inputSummary.Votes,
                Nulls = inputSummary.Nulls,
                PresencePercent = inputSummary.PresencePercent
            };

            switch (inputSummary)
            {
                case ElectionSummary electionSummary:
                    summary.PollingSections = electionSummary.PollingSections;
                    break;
                case LocalitySummary localitySummary:
                    summary.PollingSections = localitySummary.PollingSections;
                    summary.PollingSectionsNumbering = localitySummary.PollingSectionsNumbering.NormalizeName();
                    summary.Coefficient = localitySummary.Coefficient;
                    summary.Threshold = localitySummary.Threshold;
                    summary.Circumscription = localitySummary.Circumscription;
                    summary.MinVotes = localitySummary.MinVotes;
                    break;
                case SectionSummary sectionSummary:
                    summary.Address = sectionSummary.Address;
                    break;
            }

            _summaries.Add(summary);
        }

        private void AddResults(IReadOnlyCollection<Candidate> results, int idElection, string topicName, int roundName, int idDivision)
        {
            if (results == null)
            {
                return;
            }

            foreach (var result in results)
            {
                _results.Add(
                    new Result
                    {
                        IdElection = idElection,
                        Topic = topicName,
                        Round = roundName,
                        IdDivision = idDivision,
                        Name = result.Name.ToFormWithoutDiacritics(),
                        Party = result.Party.ToFormWithoutDiacritics(),
                        ShortName = result.ShortName.ToFormWithoutDiacritics(),
                        Votes = result.Votes,
                        Percent = result.Percent,
                        VotesYes = result.VotesYes,
                        PercentYes = result.PercentYes,
                        VotesNo = result.VotesNo,
                        PercentNo = result.PercentNo,
                        Mandates = result.Mandates,
                        Mandates1 = result.Mandates1,
                        Mandates2 = result.Mandates2,
                        OverElectoralThreshold = result.OverElectoralThreshold,
                        Candidates = result.Candidates
                    }
                );
            }
        }

        private Division GetDivisionFrom(string name, int idParent)
        {
            var normalizedName = name.ToTitleCase().NormalizeName();
            var division = GetExistingDivisionFrom(normalizedName, idParent);

            if (division == null)
            {
                division = new Division
                {
                    Id = ++_lastUniqueDivisionId,
                    IdParent = idParent,
                    Name = normalizedName
                };

                _divisions.Add(division);
            }

            return division;
        }

        private Division GetExistingDivisionFrom(string normalizedName, int idParent)
        {
            var cleansedName = normalizedName.ToLettersAndDigitsOnly();

            return _divisions.FirstOrDefault(
                divisionItem =>
                {
                    // this adds a lot of overhead...
                    var cleansedDivision = divisionItem.Name.ToLettersAndDigitsOnly();
                    return (
                               cleansedDivision.StartsWith(cleansedName, StringComparison.InvariantCultureIgnoreCase)
                               || cleansedName.StartsWith(cleansedDivision, StringComparison.InvariantCultureIgnoreCase)
                           )
                           && divisionItem.IdParent == idParent;
                }
            );
        }

        private static int GetRoundNameFrom(int roundId, int roundsCount)
        {
            return roundsCount > 1 ? roundId : 0;
        }

        private static string GetTopicNameFrom(Topic topic, Entities.Election election)
        {
            switch (topic.Id)
            {
                case DataConstants.MayorTopicKey:
                    return election.Category == Category.Referendum ? topic.Name.Split(" - ")[1].ToFormWithoutDiacritics() : DataConstants.MayorTopicName;
                case DataConstants.LocalCouncilTopicKey:
                    return election.Category == Category.Referendum ? topic.Name.Split(" - ")[1].ToFormWithoutDiacritics() : DataConstants.LocalCouncilTopicName;
                case DataConstants.CountyCouncilTopicKey:
                    return DataConstants.CountyCouncilTopicName;
                case DataConstants.SenateTopicKey:
                case DataConstants.SenateTopicKey2016:
                    return DataConstants.SenateTopicName;
                case DataConstants.DeputiesTopicKey:
                case DataConstants.DeputiesTopicKey2016:
                    return DataConstants.DeputiesTopicName;
                //case DataConstants.DefaultTopicKey:
                default:
                    return string.Empty;

            }
        }
    }
}