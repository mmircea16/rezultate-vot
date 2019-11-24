import React from "react";
import { ChartContainer } from "./CandidatesChart";
import { VoteMonitoring } from "./VoteMonitoring";
import {ElectionChart} from "./Chart";
import ElectionPicker from '../services/electionPicker';

import "./style.css";
import ReactGA from 'react-ga';

const trackingId = "UA-151651448-1";
ReactGA.initialize(trackingId);
ReactGA.pageview('/home');
export const HomePage = () => {

    ElectionPicker.setDefaultElectionId('prezidentiale24112019');
  return (
    <div>
      <ChartContainer />
      <VoteMonitoring />
      <ElectionChart />
    </div>
  );
};
