import React from "react";
import { ChartContainer } from "./CandidatesChart";
import { VoteMonitoring } from "./VoteMonitoring";

import "./style.css";
import ReactGA from 'react-ga';

const trackingId = "UA-151651448-1";
ReactGA.initialize(trackingId);
ReactGA.pageview('/home');
export const HomePage = () => {
    
  return (
    <div>
      <ChartContainer />
      <VoteMonitoring />
    </div>
  );
};
