using System.ComponentModel;

namespace ElectionResults.Core.Models
{
    public enum ResultsType
    {
        [Description("PROV")]
        Provisional,
        [Description("PART")]
        Partial,
        [Description("FINAL")]
        Final,
        [Description("PRESENCE")]
        Presence,
        [Description("VOTEMONITORING")]
        VoteMonitoring
    }
}