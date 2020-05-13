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
    public class ReferendumFetcher : ElectionFetcher
    {
        public ReferendumFetcher(Election election) : base(election) { }

        protected override void PopulateTopics()
        {
            var questionsTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_ref_Intrebari_Lista&parameter={Election.Id}"
            );

            var questions = DeserializeJson<List<QuestionInfo>>(questionsTask.Result);

            foreach (var question in questions)
            {
                Election.Topics.Add(
                    question.Id.ToString(),
                    new Topic
                    {
                        Id = question.Id.ToString(),
                        Name = $"{Election.Name} - {question.Name}"
                    }
                );
            }
        }

        protected override void PopulateSummary()
        {
            var electionSummariesTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_ref_National_Sumar&parameter={Election.Id}"
            );

            var electionSummaries = DeserializeJson<List<ElectionSummary>>(electionSummariesTask.Result);

            foreach (var electionSummary in electionSummaries)
            {
                Election.Topics[electionSummary.Topic].Rounds.Add(
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
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_ref_National_Voturi&parameter={Election.Id}"
            );

            var referendumsData = DeserializeJson<List<ReferendumData>>(candidatesDataTask.Result);

            foreach (var referendumData in referendumsData)
            {
                Election.Topics[referendumData.Topic].Rounds.First().Value.Results.Add(
                    new Candidate
                    {
                        VotesYes = referendumData.VotesYes,
                        PercentYes = referendumData.PercentYes,
                        VotesNo = referendumData.VotesNo,
                        PercentNo = referendumData.PercentNo
                    }
                );
            }
        }

        protected override void GetCounties()
        {
            var countiesTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_ref_Judet_Lista&parameter={Election.Id}"
            );

            var countiesData = DeserializeJson<List<CountyData>>(countiesTask.Result);

            foreach (var countyData in countiesData)
            {
                foreach (var topic in Election.Topics.Values)
                {
                    topic.Rounds.First().Value.Counties.Add(
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
            var counties = Election.Topics.First().Value.Rounds.First().Value.Counties;
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
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_ref_Judet_Sumar&parameter={Election.Id}&parameter={idCounty}"
            );

            var countiesData = DeserializeJson<List<CountySummary>>(countiesTask.Result);

            foreach (var countyData in countiesData)
            {
                var county = Election.Topics[countyData.Topic].Rounds.First().Value.Counties[idCounty];

                county.Summary.Enrolled = countyData.Enrolled;
                county.Summary.Voting = countyData.Voting;
                county.Summary.Votes = countyData.Votes;
                county.Summary.Nulls = countyData.Nulls;
                county.Summary.PresencePercent = countyData.PresencePercent;
            }
        }

        private void PopulateCountyResults(int idCounty)
        {
            var candidatesDataTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_ref_Judet_Voturi&parameter={Election.Id}&parameter={idCounty}"
            );

            var referendumsData = DeserializeJson<List<ReferendumData>>(candidatesDataTask.Result);

            foreach (var referendumData in referendumsData)
            {
                Election.Topics[referendumData.Topic].Rounds.First().Value.Counties[idCounty].Results.Add(
                    new Candidate
                    {
                        VotesYes = referendumData.VotesYes,
                        PercentYes = referendumData.PercentYes,
                        VotesNo = referendumData.VotesNo,
                        PercentNo = referendumData.PercentNo
                    }
                );
            }
        }

        private void GetCountyLocalities(int idCounty)
        {
            var localitiesTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_ref_Localitate_Lista&parameter={Election.Id}&parameter={idCounty}"
            );

            var localitiesData = DeserializeJson<List<LocalityData>>(localitiesTask.Result);

            foreach (var localityData in localitiesData)
            {
                foreach (var topic in Election.Topics.Values)
                {
                    var county = topic.Rounds.First().Value.Counties[idCounty];
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
            var localitiesTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_ref_Localitate_Sumar&parameter={Election.Id}&parameter={idLocality}"
            );

            var localitiesData = DeserializeJson<List<CountySummary>>(localitiesTask.Result);

            foreach (var localityData in localitiesData)
            {
                var locality = Election.Topics[localityData.Topic].Rounds.First().Value.Counties[idCounty].Localities[idLocality];

                locality.Summary.Enrolled = localityData.Enrolled;
                locality.Summary.Voting = localityData.Voting;
                locality.Summary.Votes = localityData.Votes;
                locality.Summary.Nulls = localityData.Nulls;
                locality.Summary.PresencePercent = localityData.PresencePercent;
            }
        }

        private void PopulateLocalityResults(int idCounty, string idLocality)
        {
            var candidatesDataTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_ref_Localitate_Voturi&parameter={Election.Id}&parameter={idLocality}"
            );

            var referendumsData = DeserializeJson<List<ReferendumData>>(candidatesDataTask.Result);

            foreach (var referendumData in referendumsData)
            {
                Election.Topics[referendumData.Topic].Rounds.First().Value.Counties[idCounty].Localities[idLocality].Results.Add(
                    new Candidate
                    {
                        VotesYes = referendumData.VotesYes,
                        PercentYes = referendumData.PercentYes,
                        VotesNo = referendumData.VotesNo,
                        PercentNo = referendumData.PercentNo
                    }
                );
            }
        }

        private void GetLocalityPollingSections(int idCounty, string idLocality)
        {
            var pollingSectionsTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_ref_Sectie_Lista&parameter={Election.Id}&parameter={idLocality}"
            );

            var pollingSectionsData = DeserializeJson<List<PollingSectionData>>(pollingSectionsTask.Result);

            foreach (var pollingSectionData in pollingSectionsData)
            {
                foreach (var topic in Election.Topics.Values)
                {
                    var locality = topic.Rounds.First().Value.Counties[idCounty].Localities[idLocality];
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
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_ref_Sectie_Sumar&parameter={Election.Id}&parameter={idCounty}&parameter={idPollingSection}"
            );

            var pollingSectionsData = DeserializeJson<List<PollingSectionSummary>>(pollingSectionsTask.Result);

            foreach (var pollingSectionData in pollingSectionsData)
            {
                var pollingSection = Election.Topics[pollingSectionData.Topic].Rounds.First().Value.Counties[idCounty].Localities[idLocality].PollingSections[idPollingSection];

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
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=v1_ref_Sectie_Voturi&parameter={Election.Id}&parameter={idCounty}&parameter={idPollingSection}"
            );

            var referendumsData = DeserializeJson<List<ReferendumData>>(candidatesDataTask.Result);

            foreach (var referendumData in referendumsData)
            {
                Election.Topics[referendumData.Topic].Rounds.First().Value.Counties[idCounty].Localities[idLocality].PollingSections[idPollingSection].Results.Add(
                    new Candidate
                    {
                        VotesYes = referendumData.VotesYes,
                        PercentYes = referendumData.PercentYes,
                        VotesNo = referendumData.VotesNo,
                        PercentNo = referendumData.PercentNo
                    }
                );
            }
        }
    }
}
