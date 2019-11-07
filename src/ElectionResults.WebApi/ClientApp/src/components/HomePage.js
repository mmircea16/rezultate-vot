import React from "react";
import { ChartContainer } from "./CandidatesChart";
import { VoteMonitoring } from "./VoteMonitoring";
import { ElectionChart } from './Chart';
import "./style.css";
import ReactGA from 'react-ga';

const trackingId = "UA-151651448-1";
ReactGA.initialize(trackingId);
ReactGA.pageview('/home');
let search = window.location.search;
let params = new URLSearchParams(search);
var admin = params.get('admin');
export const HomePage = () => {
    const [voteMonitoringData, setVoteMonitoringData] = React.useState(null);
    
  React.useEffect(() => {
    fetch("/api/results/monitoring")
      .then(data => data.json())
      .then(data => setVoteMonitoringData(data.statistics));
  }, []);
    if (!admin) {
      return (
      <p style = { {'font-size': `72px`, marginBottom: '50px' }}>Coming soon</p>);
    }
  return (
    <div>
      <ChartContainer />
      <VoteMonitoring voteMonitoringData={voteMonitoringData} />
      <ElectionChart />
    </div>
  );
};
