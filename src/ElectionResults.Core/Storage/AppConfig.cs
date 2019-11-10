namespace ElectionResults.Core.Storage
{
    public class AppConfig
    {
        public string TableName { get; set; }

        public string BucketName { get; set; }

        public string ElectionsConfig { get; set; }

        public int IntervalInSeconds { get; set; }
        
        public int TurnoutCacheIntervalInSeconds { get; set; }

        public int ResultsCacheIntervalInSeconds { get; set; }
    }
}