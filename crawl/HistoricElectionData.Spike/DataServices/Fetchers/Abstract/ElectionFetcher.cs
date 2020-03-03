using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using HistoricElectionData.Spike.Entities;
using HistoricElectionData.Spike.JsonConverters;
using HistoricElectionData.Spike.JsonSerializationEntities;
using Newtonsoft.Json;

namespace HistoricElectionData.Spike.DataServices.Fetchers.Abstract
{
    public abstract class ElectionFetcher
    {
        protected static readonly HttpClient HttpClient = new HttpClient();
        protected readonly Election Election;

        static ElectionFetcher()
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Historic Election Data Client");
        }

        protected ElectionFetcher(Election election)
        {
            Election = election;
        }

        public static List<ElectionData> GetElectionData()
        {
            var electionsTask = HttpClient.GetStringAsync("http://alegeri.roaep.ro/wp-content/plugins/aep/aep_posts.php?qType=post");

            return DeserializeJson<List<ElectionData>>(electionsTask.Result);
        }

        protected static T DeserializeJson<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input, new BooleanJsonConverter());
        }

        public void FetchElectionData()
        {
            AddElectionData();

            PopulateTopics();

            PopulateSummary();

            PopulateGeneralResults();

            GetCounties();
            PopulateCounties();

            FixElectionTopicsAndRoundsNames();
        }

        protected virtual void AddElectionData()
        {
            var electionInfoTask = HttpClient.GetStringAsync(
                $"http://alegeri.roaep.ro/wp-content/plugins/aep/aep_data.php?name=GetAlegeriById&parameter={Election.Id}"
            );

            var electionInfo = DeserializeJson<List<ElectionInfo>>(electionInfoTask.Result).First();

            Election.Date = electionInfo.Date;
            Election.Uninominal = electionInfo.Uninominal;
        }

        protected virtual void PopulateTopics()
        {
            Election.Topics.Add(
                DataConstants.DefaultTopicKey,
                new Topic
                {
                    Id = DataConstants.DefaultTopicKey,
                    Name = Election.Name
                }
            );
        }

        protected abstract void PopulateSummary();

        protected abstract void PopulateGeneralResults();

        protected abstract void GetCounties();

        protected abstract void PopulateCounties();

        private void FixElectionTopicsAndRoundsNames()
        {
            foreach (var topic in Election.Topics.Values)
            {
                if (string.IsNullOrEmpty(topic.Name))
                {
                    topic.Name = Election.Name;
                }

                if (topic.Rounds.Count > 1)
                {
                    foreach (var (roundName, roundData) in topic.Rounds)
                    {
                        roundData.Name = $"{topic.Name} - Turul {roundName}";
                    }
                }
                else
                {
                    topic.Rounds.First().Value.Name = topic.Name;
                }
            }
        }
    }
}
