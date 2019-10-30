import React from "react";
import { ChartContainer } from "./CandidatesChart";
import { VoteMonitoring } from "./VoteMonitoring";

export const HomePage = () => {
  return (
    <div>
      <ChartContainer />
      <VoteMonitoring />
    </div>
  );
};
