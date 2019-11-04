import React from "react";
import { ChartContainer } from "./CandidatesChart";
import { VoteMonitoring } from "./VoteMonitoring";
import "./style.css";

export const HomePage = () => {
  const [voteMonitoringData, setVoteMonitoringData] = React.useState(null);
  React.useEffect(() => {
    fetch("https://mv-mobile-test.azurewebsites.net/api/v1/statistics/mini/all")
      .then(data => data.json())
      .then(data => {
        console.log(data);
        setVoteMonitoringData(data);
      });
  }, []);
  return (
    <div>
      <ChartContainer />
      <VoteMonitoring voteMonitoringData={voteMonitoringData} />
    </div>
  );
};
