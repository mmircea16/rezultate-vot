namespace ElectionResults.Core.Storage
{
    public class Consts
    {
        public const string FirstElectionRound = "prezidentiale10112019";
        public const string SecondElectionRound = "prezidentiale24112019";
        public const string ResultsCountKey = "results-count";
        public const string ParameterStoreName = "vote-results";
        public const string VoteMonitoringKey = "MONITORING";
        public const string VoteTurnoutKey = "TURNOUT";
        public const string SSMServiceUrl = "http://localhost:4583";
        public const string S3ServiceUrl = "http://localhost:4572";
        public const string DynamoDbServiceUrl = "http://localhost:4569";
    }
}