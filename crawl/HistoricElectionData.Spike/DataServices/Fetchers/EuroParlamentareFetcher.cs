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
    public class EuroParlamentareFetcher : ElectionFetcher
    {
        public EuroParlamentareFetcher(Election election) : base(election) { }

        protected override void PopulateSummary()
        {
            var electionSummariesTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_euro_Tara_Sumar&parameter={Election.Id}"
            );

            var electionSummaries = DeserializeJson<List<ElectionSummary>>(electionSummariesTask.Result);

            foreach (var electionSummary in electionSummaries)
            {
                Election.Topics[DataConstants.DefaultTopicKey].Rounds.Add(
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
            }
        }

        protected override void PopulateGeneralResults()
        {
            var candidatesDataTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_euro_Tara_Voturi&parameter={Election.Id}"
            );

            var candidatesData = DeserializeJson<List<CandidateData>>(candidatesDataTask.Result);

            foreach (var candidateData in candidatesData)
            {
                Election.Topics[DataConstants.DefaultTopicKey].Rounds[candidateData.Round].Results.Add(
                    new Candidate
                    {
                        Name = candidateData.Name,
                        Party = candidateData.Party,
                        Votes = candidateData.Votes,
                        Percent = candidateData.Percent,
                        ShortName = candidateData.ShortName
                    }
                );

                Election.Topics[DataConstants.DefaultTopicKey].Rounds[candidateData.Round].Summary.Mandates += candidateData.Mandates;
            }
        }

        protected override void GetCounties()
        {
            var countiesTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_euro_Judet_Lista&parameter={Election.Id}"
            );

            var countiesData = DeserializeJson<List<CountyData>>(countiesTask.Result);

            foreach (var countyData in countiesData)
            {
                foreach (var round in Election.Topics[DataConstants.DefaultTopicKey].Rounds.Values)
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

        protected override void PopulateCounties()
        {
            var counties = Election.Topics[DataConstants.DefaultTopicKey].Rounds.First().Value.Counties;
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
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_euro_Judet_Sumar&parameter={Election.Id}&parameter={idCounty}"
            );

            var countiesData = DeserializeJson<List<CountySummary>>(countiesTask.Result);

            foreach (var countyData in countiesData)
            {
                var county = Election.Topics[DataConstants.DefaultTopicKey].Rounds[countyData.Round].Counties[idCounty];
                county.Summary.PollingSections = countyData.PollingSections;
                county.Summary.Enrolled = countyData.Enrolled;
                county.Summary.Voting = countyData.Voting;
                county.Summary.Votes = countyData.Votes;
                county.Summary.Nulls = countyData.Nulls;
                county.Summary.PollingSectionsNumbering = countyData.PollingSectionsNumbering;
                county.Summary.PresencePercent = countyData.PresencePercent;
            }
        }

        private void PopulateCountyResults(int idCounty)
        {
            var candidatesDataTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_euro_Judet_Voturi&parameter={Election.Id}&parameter={idCounty}"
            );

            var candidatesData = DeserializeJson<List<CandidateData>>(candidatesDataTask.Result);

            foreach (var candidateData in candidatesData)
            {
                var county = Election.Topics[DataConstants.DefaultTopicKey].Rounds[candidateData.Round].Counties[idCounty];
                county.Results.Add(
                    new Candidate
                    {
                        Name = candidateData.Name,
                        Party = candidateData.Party,
                        Votes = candidateData.Votes,
                        Percent = candidateData.Percent,
                        ShortName = candidateData.ShortName
                    }
                );
            }
        }

        private void GetCountyLocalities(int idCounty)
        {
            var localitiesTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_euro_Localitate_Lista&parameter={Election.Id}&parameter={idCounty}"
            );

            var localitiesData = DeserializeJson<List<LocalityData>>(localitiesTask.Result);

            foreach (var localityData in localitiesData)
            {
                foreach (var round in Election.Topics[DataConstants.DefaultTopicKey].Rounds.Keys)
                {
                    var county = Election.Topics[DataConstants.DefaultTopicKey].Rounds[round].Counties[idCounty];
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
                try
                {
                    PopulateLocalitySummary(county.Id, idLocality);
                    PopulateLocalityResults(county.Id, idLocality);
                    GetLocalityPollingSections(county.Id, idLocality);
                    PopulateLocalityPollingSections(county.Id, localities[idLocality]);
                }
                catch
                {
                    // there's buggy data - Greece Athens appears twice and the first occurrence is null - should add proper error handling here
                }
            }
        }

        private void PopulateLocalitySummary(int idCounty, string idLocality)
        {
            var localitiesTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_euro_Localitate_Sumar&parameter={Election.Id}&parameter={idLocality}"
            );

            var localitiesData = DeserializeJson<List<CountySummary>>(localitiesTask.Result);

            foreach (var localityData in localitiesData)
            {
                var locality = Election.Topics[DataConstants.DefaultTopicKey].Rounds[localityData.Round].Counties[idCounty].Localities[idLocality];

                locality.Summary.PollingSections = localityData.PollingSections;
                locality.Summary.Enrolled = localityData.Enrolled;
                locality.Summary.Voting = localityData.Voting;
                locality.Summary.Votes = localityData.Votes;
                locality.Summary.Nulls = localityData.Nulls;
                locality.Summary.PollingSectionsNumbering = localityData.PollingSectionsNumbering;
                locality.Summary.PresencePercent = localityData.PresencePercent;
            }
        }

        private void PopulateLocalityResults(int idCounty, string idLocality)
        {
            var candidatesDataTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_euro_Localitate_Voturi&parameter={Election.Id}&parameter={idLocality}"
            );

            var candidatesData = DeserializeJson<List<CandidateData>>(candidatesDataTask.Result);

            foreach (var candidateData in candidatesData)
            {
                var locality = Election.Topics[DataConstants.DefaultTopicKey].Rounds[candidateData.Round].Counties[idCounty].Localities[idLocality];

                locality.Results.Add(
                    new Candidate
                    {
                        Name = candidateData.Name,
                        Party = candidateData.Party,
                        Votes = candidateData.Votes,
                        Percent = candidateData.Percent,
                        ShortName = candidateData.ShortName
                    }
                );
            }
        }

        private void GetLocalityPollingSections(int idCounty, string idLocality)
        {
            var pollingSectionsTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_euro_Sectie_Lista&parameter={Election.Id}&parameter={idLocality}"
            );

            var pollingSectionsData = DeserializeJson<List<PollingSectionData>>(pollingSectionsTask.Result);

            foreach (var pollingSectionData in pollingSectionsData)
            {
                foreach (var round in Election.Topics[DataConstants.DefaultTopicKey].Rounds.Keys)
                {
                    var locality = Election.Topics[DataConstants.DefaultTopicKey].Rounds[round].Counties[idCounty].Localities[idLocality];
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
            var pollingSectionsTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_euro_Sectie_Sumar&parameter={Election.Id}&parameter={idCounty}&parameter={idPollingSection}"
            );

            var pollingSectionsData = DeserializeJson<List<PollingSectionSummary>>(pollingSectionsTask.Result);

            foreach (var pollingSectionData in pollingSectionsData)
            {
                var pollingSection = Election.Topics[DataConstants.DefaultTopicKey].Rounds[pollingSectionData.Round].Counties[idCounty].Localities[idLocality].PollingSections[idPollingSection];

                pollingSection.Summary.Enrolled = pollingSectionData.Enrolled;
                pollingSection.Summary.Voting = pollingSectionData.Voting;
                pollingSection.Summary.Votes = pollingSectionData.Votes;
                pollingSection.Summary.Nulls = pollingSectionData.Nulls;
                pollingSection.Summary.Address = pollingSectionData.Address;
                pollingSection.Summary.PresencePercent = pollingSectionData.PresencePercent;
            }
        }

        private void PopulatePollingSectionResults(int idCounty, string idLocality, int idPollingSection)
        {
            var candidatesDataTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_euro_Sectie_Voturi&parameter={Election.Id}&parameter={idCounty}&parameter={idPollingSection}"
            );

            var candidatesData = DeserializeJson<List<CandidateData>>(candidatesDataTask.Result);

            foreach (var candidateData in candidatesData)
            {
                var pollingSection = Election.Topics[DataConstants.DefaultTopicKey].Rounds[candidateData.Round].Counties[idCounty].Localities[idLocality].PollingSections[idPollingSection];

                pollingSection.Results.Add(
                    new Candidate
                    {
                        Name = candidateData.Name,
                        Party = candidateData.Party,
                        Votes = candidateData.Votes,
                        Percent = candidateData.Percent,
                        ShortName = candidateData.ShortName
                    }
                );
            }
        }
    }
}
