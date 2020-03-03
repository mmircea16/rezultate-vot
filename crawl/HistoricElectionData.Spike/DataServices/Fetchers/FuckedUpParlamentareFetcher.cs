using System;
using System.Collections.Generic;
using HistoricElectionData.Spike.DataServices.Fetchers.Abstract;
using HistoricElectionData.Spike.Entities;
using HistoricElectionData.Spike.Entities.Results;
using HistoricElectionData.Spike.JsonSerializationEntities;
using HistoricElectionData.Spike.JsonSerializationEntities.Lists;
using HistoricElectionData.Spike.JsonSerializationEntities.Summaries;

namespace HistoricElectionData.Spike.DataServices.Fetchers
{
    public class FuckedUpParlamentareFetcher : ElectionFetcher
    {
        public FuckedUpParlamentareFetcher(Election election) : base(election) { }

        protected override void AddElectionData()
        {
            Election.Id = 249;
            Election.Date = new DateTime(2016, 12, 11);
            Election.Uninominal = false;
        }

        protected override void PopulateTopics()
        {
            Election.Topics.Add(
                DataConstants.FuckedUpSenateTopicKey,
                new Topic
                {
                    Id = DataConstants.FuckedUpSenateTopicKey,
                    Name = $"{Election.Name} - {DataConstants.SenateTopicName}"
                }
            );

            Election.Topics.Add(
                DataConstants.FuckedUpDeputiesTopicKey,
                new Topic
                {
                    Id = DataConstants.FuckedUpDeputiesTopicKey,
                    Name = $"{Election.Name} - {DataConstants.DeputiesTopicName}"
                }
            );
        }

        protected override void PopulateSummary()
        {
            foreach (var (idTopic, topic) in Election.Topics)
            {
                var electionSummariesTask = HttpClient.GetStringAsync(
                    $"http://213.177.15.7:8080/siap-wp/rest/aep_data.jsp?name=v1_parl_Tara_Sumar&parameter={Election.Id}&parameter={idTopic}"
                );

                var electionSummaries = DeserializeJson<List<ElectionSummary>>(electionSummariesTask.Result);

                foreach (var electionSummary in electionSummaries)
                {
                    topic.Rounds.Add(
                        0,
                        new Round
                        {
                            Id = 0,
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
                }
            }
        }

        protected override void PopulateGeneralResults()
        {
            foreach (var (idTopic, topic) in Election.Topics)
            {
                var candidatesDataTask = HttpClient.GetStringAsync(
                    $"http://213.177.15.7:8080/siap-wp/rest/aep_data.jsp?name=v1_parl_Tara_Voturi&parameter={Election.Id}&parameter={idTopic}"
                );

                var candidatesData = DeserializeJson<List<CandidateData>>(candidatesDataTask.Result);

                foreach (var candidateData in candidatesData)
                {
                    topic.Rounds[0].Results.Add(
                        new Candidate
                        {
                            Party = candidateData.Party,
                            ShortName = candidateData.ShortName,
                            Votes = candidateData.Votes,
                            Percent = candidateData.Percent
                        }
                    );
                }
            }
        }

        protected override void GetCounties()
        {
            var countiesTask = HttpClient.GetStringAsync(
                $"http://213.177.15.7:8080/siap-wp/rest/aep_data.jsp?name=v1_parl_Judet_Lista&parameter={Election.Id}&parameter={DataConstants.FuckedUpSenateTopicKey}"
            );

            var countiesData = DeserializeJson<List<CountyData>>(countiesTask.Result);

            foreach (var countyData in countiesData)
            {
                foreach (var topic in Election.Topics.Values)
                {
                    topic.Rounds[0].Counties.Add(
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

        protected override void PopulateCounties()
        {
            var counties = Election.Topics[DataConstants.FuckedUpSenateTopicKey].Rounds[0].Counties;
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
            foreach (var (idTopic, topic) in Election.Topics)
            {
                var countiesTask = HttpClient.GetStringAsync(
                    $"http://213.177.15.7:8080/siap-wp/rest/aep_data.jsp?name=v1_parl_Judet_Sumar&parameter={Election.Id}&parameter={idCounty}&parameter={idTopic}"
                );

                var countiesData = DeserializeJson<List<CountySummary>>(countiesTask.Result);

                foreach (var countyData in countiesData)
                {
                    var county = topic.Rounds[0].Counties[idCounty];
                    county.Summary.PollingSections = countyData.PollingSections;
                    county.Summary.Enrolled = countyData.Enrolled;
                    county.Summary.Voting = countyData.Voting;
                    county.Summary.Votes = countyData.Votes;
                    county.Summary.Nulls = countyData.Nulls;
                    county.Summary.PollingSectionsNumbering = countyData.PollingSectionsNumbering;
                    county.Summary.PresencePercent = countyData.PresencePercent;
                }
            }
        }

        private void PopulateCountyResults(int idCounty)
        {
            foreach (var (idTopic, topic) in Election.Topics)
            {
                var candidatesDataTask = HttpClient.GetStringAsync(
                    $"http://213.177.15.7:8080/siap-wp/rest/aep_data.jsp?name=v1_parl_Judet_Voturi&parameter={Election.Id}&parameter={idCounty}&parameter={idTopic}"
                );

                var candidatesData = DeserializeJson<List<CandidateData>>(candidatesDataTask.Result);

                foreach (var candidateData in candidatesData)
                {
                    var county = topic.Rounds[0].Counties[idCounty];
                    county.Results.Add(
                        new Candidate
                        {
                            Party = candidateData.Party,
                            ShortName = candidateData.ShortName,
                            Votes = candidateData.Votes,
                            Percent = candidateData.Percent
                        }
                    );
                }
            }
        }

        private void GetCountyLocalities(int idCounty)
        {
            var localitiesTask = HttpClient.GetStringAsync(
                $"http://213.177.15.7:8080/siap-wp/rest/aep_data.jsp?name=v1_parl_Localitate_Lista&parameter={Election.Id}&parameter={idCounty}"
            );

            var localitiesData = DeserializeJson<List<LocalityData>>(localitiesTask.Result);

            foreach (var localityData in localitiesData)
            {
                foreach (var topic in Election.Topics.Values)
                {
                    var county = topic.Rounds[0].Counties[idCounty];
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
                        $"http://213.177.15.7:8080/siap-wp/rest/aep_data.jsp?name=v1_parl_Localitate_Sumar&parameter={Election.Id}&parameter={idLocality}&parameter={idTopic}"
                );

                var localitiesData = DeserializeJson<List<CountySummary>>(localitiesTask.Result);

                foreach (var localityData in localitiesData)
                {

                    var locality = topic.Rounds[0].Counties[idCounty].Localities[idLocality];

                    locality.Summary.PollingSections = localityData.PollingSections;
                    locality.Summary.Enrolled = localityData.Enrolled;
                    locality.Summary.Voting = localityData.Voting;
                    locality.Summary.Votes = localityData.Votes;
                    locality.Summary.Nulls = localityData.Nulls;
                    locality.Summary.PollingSectionsNumbering = localityData.PollingSectionsNumbering;
                    locality.Summary.PresencePercent = localityData.PresencePercent;
                }
            }
        }

        private void PopulateLocalityResults(int idCounty, string idLocality)
        {
            foreach (var (idTopic, topic) in Election.Topics)
            {
                var candidatesDataTask = HttpClient.GetStringAsync(
                    $"http://213.177.15.7:8080/siap-wp/rest/aep_data.jsp?name=v1_parl_Localitate_Voturi&parameter={Election.Id}&parameter={idLocality}&parameter={idTopic}"
                );

                var candidatesData = DeserializeJson<List<CandidateData>>(candidatesDataTask.Result);

                foreach (var candidateData in candidatesData)
                {
                    var locality = topic.Rounds[0].Counties[idCounty].Localities[idLocality];

                    locality.Results.Add(
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

        private void GetLocalityPollingSections(int idCounty, string idLocality)
        {
            var pollingSectionsTask = HttpClient.GetStringAsync(
                $"http://213.177.15.7:8080/siap-wp/rest/aep_data.jsp?name=v1_parl_Sectie_Lista&parameter={Election.Id}&parameter={idLocality}"
            );

            var pollingSectionsData = DeserializeJson<List<PollingSectionData>>(pollingSectionsTask.Result);

            foreach (var pollingSectionData in pollingSectionsData)
            {
                foreach (var topic in Election.Topics.Values)
                {
                    var locality = topic.Rounds[0].Counties[idCounty].Localities[idLocality];

                    if (!topic.Rounds[0].Counties[idCounty].Localities[idLocality].PollingSections.ContainsKey(pollingSectionData.Id))
                    {
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
                var summaryKey = topic.Id.Equals(DataConstants.FuckedUpSenateTopicKey) ? DataConstants.SenateSummaryKey : DataConstants.DeputiesSummaryKey;

                var pollingSectionsTask = HttpClient.GetStringAsync(
                    $"http://213.177.15.7:8080/siap-wp/rest/aep_data.jsp?name=v1_parl_Sectie_Sumar&parameter={Election.Id}&parameter={idCounty}&parameter={idPollingSection}&parameter={idTopic}&parameter={summaryKey}"
                );

                var pollingSectionsData = DeserializeJson<List<PollingSectionSummary>>(pollingSectionsTask.Result);

                foreach (var pollingSectionData in pollingSectionsData)
                {
                    var pollingSection = topic.Rounds[0].Counties[idCounty].Localities[idLocality].PollingSections[idPollingSection];

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
                    $"http://213.177.15.7:8080/siap-wp/rest/aep_data.jsp?name=v1_parl_Sectie_Voturi&parameter={Election.Id}&parameter={idCounty}&parameter={idPollingSection}&parameter={idTopic}"
                );

                var candidatesData = DeserializeJson<List<CandidateData>>(candidatesDataTask.Result);

                foreach (var candidateData in candidatesData)
                {
                    var pollingSection = topic.Rounds[0].Counties[idCounty].Localities[idLocality].PollingSections[idPollingSection];

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
