using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ElectionResults.Core.Models;
using ElectionResults.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ElectionResults.Core.Storage
{
    public class ResultsRepository : IResultsRepository
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly ILogger<ResultsRepository> _logger;
        private readonly AppConfig _config;

        public ResultsRepository(IOptions<AppConfig> config, IAmazonDynamoDB dynamoDb, ILogger<ResultsRepository> logger)
        {
            _dynamoDb = dynamoDb;
            _logger = logger;
            _config = config.Value;
        }

        public async Task InsertResults(ElectionStatistics electionStatistics)
        {
            try
            {
                _logger.LogInformation($"Inserting results");

                Dictionary<string, AttributeValue> item = new Dictionary<string, AttributeValue>();
                item["id"] = new AttributeValue { S = electionStatistics.Id };
                item["csvType"] = new AttributeValue { S = electionStatistics.Type };
                item["csvLocation"] = new AttributeValue { S = electionStatistics.Location };
                item["statisticsJson"] = new AttributeValue { S = electionStatistics.StatisticsJson };
                item["csvTimestamp"] = new AttributeValue { N = electionStatistics.Timestamp.ToString() };
                List<WriteRequest> items = new List<WriteRequest>();
                items.Add(new WriteRequest
                {
                    PutRequest = new PutRequest { Item = item }
                });
                Dictionary<string, List<WriteRequest>> requestItems = new Dictionary<string, List<WriteRequest>>();
                requestItems[_config.TableName] = items;

                await _dynamoDb.BatchWriteItemAsync(requestItems);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Couldn't insert results");
            }
        }

        public async Task<ElectionStatistics> GetLatestResults(string location, string type)
        {
            var queryRequest = new QueryRequest(_config.TableName);
            queryRequest.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {":csvType", new AttributeValue(type)},
                {":csvLocation", new AttributeValue(location) }
            };
            queryRequest.IndexName = "latest-result";
            queryRequest.KeyConditionExpression = "csvType = :csvType and csvLocation = :csvLocation";
            var queryResponse = await _dynamoDb.QueryAsync(queryRequest);

            var results = GetResults(queryResponse.Items);
            var latest = results.OrderByDescending(r => r.Timestamp).FirstOrDefault();
            _logger.LogInformation($"Latest for {type} and {location} is {latest.Timestamp}");
            return latest;
        }

        public virtual async Task InitializeDb()
        {
            var tableExists = await TableExists();
            if (!tableExists)
                await CreateTable();
        }

        public virtual async Task InsertCurrentVoterTurnout(VoterTurnout voterTurnout)
        {
            var electionStatistics = new ElectionStatistics
            {
                Id = $"{DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks:D19}",
                Type = ResultsType.VoterTurnout.ConvertEnumToString(),
                Location = ResultsLocation.All.ConvertEnumToString(),
                Timestamp = voterTurnout.Timestamp,
                StatisticsJson = JsonConvert.SerializeObject(voterTurnout)
            };
            await InsertResults(electionStatistics);
        }

        public async Task InsertVoteMonitoringStats(VoteMonitoringStats voteMonitoringInfo)
        {
            var electionStatistics = new ElectionStatistics
            {
                Id = $"{DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks:D19}",
                Type = ResultsType.VoteMonitoring.ConvertEnumToString(),
                Location = ResultsLocation.All.ConvertEnumToString(),
                Timestamp = voteMonitoringInfo.Timestamp,
                StatisticsJson = JsonConvert.SerializeObject(voteMonitoringInfo)
            };
            await InsertResults(electionStatistics);
        }

        private async Task WaitUntilTableReady(string tableName)
        {
            string status = null;

            do
            {
                await Task.Delay(2000);
                try
                {
                    var res = _dynamoDb.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = tableName
                    });

                    status = res.Result.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {

                }

            } while (status != "ACTIVE");
        }

        private async Task<bool> TableExists()
        {
            try
            {
                var res = await _dynamoDb.DescribeTableAsync(new DescribeTableRequest
                {
                    TableName = _config.TableName
                });
                return res.Table.TableStatus == "ACTIVE";
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Table {_config.TableName} isn't ready'");
                return false;
            }
        }

        private List<ElectionStatistics> GetResults(List<Dictionary<string, AttributeValue>> foundItems)
        {
            return foundItems.Select(item => new ElectionStatistics
            {
                Id = item["id"].S,
                Timestamp = Convert.ToInt64(item["csvTimestamp"].N),
                StatisticsJson = item["statisticsJson"].S,
                Location = item["csvLocation"].S,
                Type = item["csvType"].S

            }).ToList();
        }

        private async Task CreateTable()
        {
            Console.WriteLine("Creating Table");

            try
            {
                var request = new CreateTableRequest
                {
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "id",
                            AttributeType = ScalarAttributeType.S
                        },
                        new AttributeDefinition
                        {
                            AttributeName = "csvType",
                            AttributeType = ScalarAttributeType.S
                        },
                        new AttributeDefinition
                        {
                            AttributeName = "csvLocation",
                            AttributeType = ScalarAttributeType.S
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "id",
                            KeyType = KeyType.HASH
                        },
                        new KeySchemaElement
                        {
                            AttributeName = "csvType",
                            KeyType = KeyType.RANGE
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = 5
                    },
                    TableName = _config.TableName,
                    GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
                    {
                        new GlobalSecondaryIndex
                        {
                            IndexName = "latest-result",
                            KeySchema = new List<KeySchemaElement>
                            {
                                new KeySchemaElement("csvLocation", KeyType.HASH),
                                new KeySchemaElement("csvType", KeyType.RANGE)
                            },
                            Projection = new Projection{ProjectionType = ProjectionType.ALL},
                            ProvisionedThroughput = new ProvisionedThroughput(5, 5)
                        }
                    }
                };

                var response = await _dynamoDb.CreateTableAsync(request);
                await WaitUntilTableReady(_config.TableName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}