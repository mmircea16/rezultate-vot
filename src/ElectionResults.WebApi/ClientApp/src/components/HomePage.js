import React from "react";
import { ChartContainer } from "./CandidatesChart";
import { VoteMonitoring } from "./VoteMonitoring";

import "./style.css";

export const HomePage = () => {
  const [voteMonitoringData, setVoteMonitoringData] = React.useState(null);

  React.useEffect(() => {
    fetch("/api/results/monitoring")
      .then(data => data.json())
      .then(data => setVoteMonitoringData(data.statistics));
  }, []);

  return (
    <div>
      <ChartContainer />
      <VoteMonitoring voteMonitoringData={voteMonitoringData} />
    </div>
  );
};
