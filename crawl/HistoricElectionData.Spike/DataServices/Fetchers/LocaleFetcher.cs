using System.Collections.Generic;
using System.Linq;
using HistoricElectionData.Spike.DataServices.Fetchers.Abstract;
using HistoricElectionData.Spike.Entities;
using HistoricElectionData.Spike.Entities.Results;
using HistoricElectionData.Spike.JsonSerializationEntities;
using HistoricElectionData.Spike.JsonSerializationEntities.Lists;
using HistoricElectionData.Spike.JsonSerializationEntities.Summaries;

namespace HistoricElectionData.Spike.DataServices.Fetchers
{
    public class LocaleFetcher : ElectionFetcher
    {
        public LocaleFetcher(Election election) : base(election) { }

        protected override void PopulateTopics()
        {
            Election.Topics.Add(
                DataConstants.MayorTopicKey,
                new Topic
                {
                    Id = DataConstants.MayorTopicKey,
                    Name = $"{Election.Name} - {DataConstants.MayorTopicName}"
                }
            );

            Election.Topics.Add(
                DataConstants.LocalCouncilTopicKey,
                new Topic
                {
                    Id = DataConstants.LocalCouncilTopicKey,
                    Name = $"{Election.Name} - {DataConstants.LocalCouncilTopicName}"
                }
            );

            Election.Topics.Add(
                DataConstants.CountyCouncilTopicKey,
                new Topic
                {
                    Id = DataConstants.CountyCouncilTopicKey,
                    Name = $"{Election.Name} - {DataConstants.CountyCouncilTopicName}"
                }
            );
        }

        protected override void PopulateSummary()
        {
            var electionSummariesTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_local_National_Sumar&parameter={Election.Id}&parameter={DataConstants.CountyCouncilTopicKey}"
            );

            var electionSummaries = DeserializeJson<List<ElectionSummary>>(electionSummariesTask.Result);

            foreach (var electionSummary in electionSummaries)
            {
                Election.Topics[DataConstants.CountyCouncilTopicKey].Rounds.Add(
                    electionSummary.Round,
                    new Round
                    {
                        Id = electionSummary.Round,
                        Summary =
                        {
                            PollingSections = electionSummary.PollingSections,
                            Enrolled = electionSummary.Enrolled,
                            Voting = electionSummary.Voting,
                            Votes = electionSummary.Votes,
                            Nulls = electionSummary.Nulls,
                            PresencePercent = electionSummary.PresencePercent
                        }
                    }
                );

                Election.Topics[DataConstants.LocalCouncilTopicKey].Rounds.Add(
                    electionSummary.Round,
                    new Round
                    {
                        Id = electionSummary.Round
                    }
                );

                Election.Topics[DataConstants.MayorTopicKey].Rounds.Add(
                    electionSummary.Round,
                    new Round
                    {
                        Id = electionSummary.Round
                    }
                );
            }
        }

        protected override void PopulateGeneralResults()
        {
            var candidatesDataTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_local_National_Voturi&parameter={Election.Id}&parameter={DataConstants.CountyCouncilTopicKey}"
            );

            var candidatesData = DeserializeJson<List<CandidateData>>(candidatesDataTask.Result);

            foreach (var candidateData in candidatesData)
            {
                Election.Topics[DataConstants.CountyCouncilTopicKey].Rounds[candidateData.Round].Results.Add(
                    new Candidate
                    {
                        Name = candidateData.Name,
                        Party = candidateData.Party,
                        Votes = candidateData.Votes,
                        Percent = candidateData.Percent,
                        Mandates = candidateData.Mandates,
                        Mandates1 = candidateData.Mandates1,
                        Mandates2 = candidateData.Mandates2,
                        OverElectoralThreshold = candidateData.OverElectoralThreshold,
                        Candidates = candidateData.Candidates
                    }
                );

                Election.Topics[DataConstants.CountyCouncilTopicKey].Rounds[candidateData.Round].Summary.Mandates += candidateData.Mandates;
            }
        }

        protected override void GetCounties()
        {
            var countiesTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_local_Judet_Lista&parameter={Election.Id}"
            );

            var countiesData = DeserializeJson<List<CountyData>>(countiesTask.Result);

            foreach (var countyData in countiesData)
            {
                foreach (var topic in Election.Topics.Values)
                {
                    foreach (var round in topic.Rounds.Values)
                    {
                        round.Counties.Add(
                            countyData.Id,
                            new County
                            {
                                Id = countyData.Id,
                                Name = countyData.Name
                            }
                        );
                    }
                }
            }
        }

        protected override void PopulateCounties()
        {
            var counties = Election.Topics[DataConstants.CountyCouncilTopicKey].Rounds.First().Value.Counties;
            foreach (var idCounty in counties.Keys)
            {
                PopulateCountySummary(idCounty);
                PopulateCountyResults(idCounty);
                GetCountyLocalities(idCounty);
                PopulateCountyLocalities(counties[idCounty]);
            }
        }

        private void PopulateCountySummary(int idCounty)
        {
            var countiesTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_local_Judet_Sumar&parameter={Election.Id}&parameter={idCounty}&parameter={DataConstants.CountyCouncilTopicKey}"
            );

            var countiesData = DeserializeJson<List<CountySummary>>(countiesTask.Result);

            foreach (var countyData in countiesData)
            {
                var county = Election.Topics[DataConstants.CountyCouncilTopicKey].Rounds[countyData.Round].Counties[idCounty];
                county.Summary.PollingSections = countyData.PollingSections;
                county.Summary.Enrolled = countyData.Enrolled;
                county.Summary.Voting = countyData.Voting;
                county.Summary.Votes = countyData.Votes;
                county.Summary.Nulls = countyData.Nulls;
                county.Summary.PollingSectionsNumbering = countyData.PollingSectionsNumbering;
                county.Summary.PresencePercent = countyData.PresencePercent;
                county.Summary.Mandates = countyData.Mandates;
                county.Summary.Coefficient = countyData.Coefficient;
                county.Summary.Threshold = countyData.Threshold;
                county.Summary.Circumscription = countyData.Circumscription;
                county.Summary.MinVotes = countyData.MinVotes;
            }
        }

        private void PopulateCountyResults(int idCounty)
        {
            var candidatesDataTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_local_Judet_Voturi&parameter={Election.Id}&parameter={idCounty}&parameter={DataConstants.CountyCouncilTopicKey}"
            );

            var candidatesData = DeserializeJson<List<CandidateData>>(candidatesDataTask.Result);

            foreach (var candidateData in candidatesData)
            {
                var county = Election.Topics[DataConstants.CountyCouncilTopicKey].Rounds[candidateData.Round].Counties[idCounty];
                county.Results.Add(
                    new Candidate
                    {
                        Name = candidateData.Name,
                        Party = candidateData.Party,
                        Votes = candidateData.Votes,
                        Percent = candidateData.Percent,
                        Mandates = candidateData.Mandates,
                        Mandates1 = candidateData.Mandates1,
                        Mandates2 = candidateData.Mandates2,
                        OverElectoralThreshold = candidateData.OverElectoralThreshold,
                        Candidates = candidateData.Candidates
                    }
                );
            }
        }

        private void GetCountyLocalities(int idCounty)
        {
            var localitiesTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_local_Localitate_Lista&parameter={Election.Id}&parameter={idCounty}"
            );

            var localitiesData = DeserializeJson<List<LocalityData>>(localitiesTask.Result);

            foreach (var localityData in localitiesData)
            {
                foreach (var (idTopic, topic) in Election.Topics)
                {
                    foreach (var idRound in topic.Rounds.Keys)
                    {
                        if (!Election.Topics[idTopic].Rounds[idRound].Counties.ContainsKey(idCounty))
                        {
                            var existingRound = topic.Rounds.First().Value;

                            Election.Topics[idTopic].Rounds[idRound].Counties.Add(
                                idCounty,
                                new County
                                {
                                    Id = idCounty,
                                    Name = existingRound.Counties[idCounty].Name
                                }
                            );
                        }

                        var county = Election.Topics[idTopic].Rounds[idRound].Counties[idCounty];
                        county.Localities.Add(
                            localityData.Id,
                            new Locality
                            {
                                Id = localityData.Id,
                                Name = localityData.Name
                            }
                        );
                    }
                }
            }
        }

        private void PopulateCountyLocalities(County county)
        {
            var localities = county.Localities;
            foreach (var idLocality in localities.Keys)
            {
                PopulateLocalitySummary(county.Id, idLocality);
                PopulateLocalityResults(county.Id, idLocality);
                GetLocalityPollingSections(county.Id, idLocality);
                PopulateLocalityPollingSections(county.Id, localities[idLocality]);
            }
        }

        private void PopulateLocalitySummary(int idCounty, string idLocality)
        {
            foreach (var (idTopic, topic) in Election.Topics)
            {
                var localitiesTask = HttpClient.GetStringAsync(
                        $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_local_Localitate_Sumar&parameter={Election.Id}&parameter={idLocality}&parameter={idTopic}"
                );

                var localitiesData = DeserializeJson<List<CountySummary>>(localitiesTask.Result);

                foreach (var localityData in localitiesData)
                {
                    if (!topic.Rounds.ContainsKey(localityData.Round))
                    {
                        topic.Rounds.Add(
                            localityData.Round,
                            new Round
                            {
                                Id = localityData.Round
                            }
                        );
                    }

                    if (!topic.Rounds[localityData.Round].Counties.ContainsKey(idCounty))
                    {
                        var existingRound = topic.Rounds.First().Value;

                        topic.Rounds[localityData.Round].Counties.Add(
                            idCounty,
                            new County
                            {
                                Id = idCounty,
                                Name = existingRound.Counties[idCounty].Name
                            }
                        );
                    }

                    if (!topic.Rounds[localityData.Round].Counties[idCounty].Localities.ContainsKey(idLocality))
                    {
                        var existingRound = topic.Rounds.First().Value;

                        topic.Rounds[localityData.Round].Counties[idCounty].Localities.Add(
                            idLocality,
                            new Locality
                            {
                                Id = idLocality,
                                Name = existingRound.Counties[idCounty].Localities[idLocality].Name
                            }
                        );
                    }

                    var locality = topic.Rounds[localityData.Round].Counties[idCounty].Localities[idLocality];

                    locality.Summary.PollingSections = localityData.PollingSections;
                    locality.Summary.Enrolled = localityData.Enrolled;
                    locality.Summary.Voting = localityData.Voting;
                    locality.Summary.Votes = localityData.Votes;
                    locality.Summary.Nulls = localityData.Nulls;
                    locality.Summary.PollingSectionsNumbering = localityData.PollingSectionsNumbering;
                    locality.Summary.PresencePercent = localityData.PresencePercent;
                    locality.Summary.Mandates = localityData.Mandates;
                    locality.Summary.Coefficient = localityData.Coefficient;
                    locality.Summary.Threshold = localityData.Threshold;
                    locality.Summary.Circumscription = localityData.Circumscription;
                    locality.Summary.MinVotes = localityData.MinVotes;
                }
            }
        }

        private void PopulateLocalityResults(int idCounty, string idLocality)
        {
            foreach (var (idTopic, topic) in Election.Topics)
            {
                var candidatesDataTask = HttpClient.GetStringAsync(
                    $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_local_Localitate_Voturi&parameter={Election.Id}&parameter={idLocality}&parameter={idTopic}"
                );

                var candidatesData = DeserializeJson<List<CandidateData>>(candidatesDataTask.Result);

                foreach (var candidateData in candidatesData)
                {
                    var locality = topic.Rounds[candidateData.Round].Counties[idCounty].Localities[idLocality];

                    locality.Results.Add(
                        new Candidate
                        {
                            Name = candidateData.Name,
                            Party = candidateData.Party,
                            Votes = candidateData.Votes,
                            Percent = candidateData.Percent,
                            Mandates = candidateData.Mandates,
                            Mandates1 = candidateData.Mandates1,
                            Mandates2 = candidateData.Mandates2,
                            OverElectoralThreshold = candidateData.OverElectoralThreshold,
                            Candidates = candidateData.Candidates
                        }
                    );
                }
            }
        }

        private void GetLocalityPollingSections(int idCounty, string idLocality)
        {
            var pollingSectionsTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_local_Sectie_Lista&parameter={Election.Id}&parameter={idLocality}"
            );

            var pollingSectionsData = DeserializeJson<List<PollingSectionData>>(pollingSectionsTask.Result);

            foreach (var pollingSectionData in pollingSectionsData)
            {
                foreach (var (idTopic, topic) in Election.Topics)
                {
                    foreach (var idRound in topic.Rounds.Keys)
                    {
                        if (!Election.Topics[idTopic].Rounds[idRound].Counties[idCounty].Localities.ContainsKey(idLocality))
                        {
                            var existingRound = topic.Rounds.First().Value;

                            Election.Topics[idTopic].Rounds[idRound].Counties[idCounty].Localities.Add(
                                idLocality,
                                new Locality
                                {
                                    Id = idLocality,
                                    Name = existingRound.Counties[idCounty].Localities[idLocality].Name
                                }
                            );
                        }

                        var locality = Election.Topics[idTopic].Rounds[idRound].Counties[idCounty].Localities[idLocality];
                        locality.PollingSections.Add(
                            pollingSectionData.Id,
                            new PollingSection
                            {
                                Id = pollingSectionData.Id,
                                Name = pollingSectionData.Name
                            }
                        );
                    }
                }
            }
        }

        private void PopulateLocalityPollingSections(int idCounty, Locality locality)
        {
            var pollingSections = locality.PollingSections;
            foreach (var idPollingSection in pollingSections.Keys)
            {
                PopulatePollingSectionSummary(idCounty, locality.Id, idPollingSection);
                PopulatePollingSectionResults(idCounty, locality.Id, idPollingSection);
            }
        }

        private void PopulatePollingSectionSummary(int idCounty, string idLocality, int idPollingSection)
        {
            foreach (var (idTopic, topic) in Election.Topics)
            {
                var pollingSectionsTask = HttpClient.GetStringAsync(
                    $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_local_Sectie_Sumar&parameter={Election.Id}&parameter={idCounty}&parameter={idPollingSection}&parameter={idTopic}&parameter={idLocality}"
                );

                var pollingSectionsData = DeserializeJson<List<PollingSectionSummary>>(pollingSectionsTask.Result);

                foreach (var pollingSectionData in pollingSectionsData)
                {
                    var pollingSection = topic.Rounds[pollingSectionData.Round].Counties[idCounty].Localities[idLocality].PollingSections[idPollingSection];

                    pollingSection.Summary.Enrolled = pollingSectionData.Enrolled;
                    pollingSection.Summary.Voting = pollingSectionData.Voting;
                    pollingSection.Summary.Votes = pollingSectionData.Votes;
                    pollingSection.Summary.Nulls = pollingSectionData.Nulls;
                    pollingSection.Summary.Address = pollingSectionData.Address;
                    pollingSection.Summary.PresencePercent = pollingSectionData.PresencePercent;
                }
            }
        }

        private void PopulatePollingSectionResults(int idCounty, string idLocality, int idPollingSection)
        {
            foreach (var (idTopic, topic) in Election.Topics)
            {
                var candidatesDataTask = HttpClient.GetStringAsync(
                    $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_local_Sectie_Voturi&parameter={Election.Id}&parameter={idCounty}&parameter={idPollingSection}&parameter={idTopic}&parameter={idLocality}"
                );

                var candidatesData = DeserializeJson<List<CandidateData>>(candidatesDataTask.Result);

                foreach (var candidateData in candidatesData)
                {
                    var pollingSection = topic.Rounds[candidateData.Round].Counties[idCounty].Localities[idLocality].PollingSections[idPollingSection];

                    pollingSection.Results.Add(
                        new Candidate
                        {
                            Name = candidateData.Name,
                            Party = candidateData.Party,
                            Votes = candidateData.Votes,
                            Percent = candidateData.Percent
                        }
                    );
                }
            }
        }
    }
}
