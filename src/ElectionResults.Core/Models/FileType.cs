using System.ComponentModel;

namespace ElectionResults.Core.Models
{
    public enum FileType
    {
        [Description("RESULTS")]
        Results,
        [Description("TURNOUT")]
        VoterTurnout,
        [Description("MONITORING")]
        VoteMonitoring
    }
}