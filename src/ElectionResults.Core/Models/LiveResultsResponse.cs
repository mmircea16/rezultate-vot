﻿using System.Collections.Generic;
using ElectionResults.Core.Services;

namespace ElectionResults.Core.Models
{
    public class LiveResultsResponse
    {
        public List<CandidateModel> Candidates { get; set; }

        public List<County> Counties { get; set; }
        public VoterTurnout VoterTurnout { get; set; }
    }
}