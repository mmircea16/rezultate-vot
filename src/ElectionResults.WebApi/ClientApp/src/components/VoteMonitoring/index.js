import React from "react";
import "./style.css";
import * as signalR from "@aspnet/signalr";
import { ElectionChart } from "../Chart";

const NumberArea = ({ bigNumber, text }) => (
  <div className={"vote-monitoring-area"}>
    <p className={"area-number"}>{bigNumber}</p>
    <p className={"area-text"}>{text}</p>
  </div>
);
export const VoteMonitoring = () => {
  const [voteMonitoringData, setVoteMonitoringData] = React.useState(null);
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/live-results")
    .build();

  React.useEffect(() => {
    fetch("/api/results/monitoring")
      .then(data => data.json())
      .then(data => setVoteMonitoringData(data.statistics));
    connection
      .start()
      .then(() => console.log("Connection started!"))
      .catch(err => console.log("Error while establishing connection :("));

    connection.on("monitoring-updated", data => {
      console.log("received statistics");
      setVoteMonitoringData(data.statistics);
    });
  }, []);

  if (!voteMonitoringData) {
    return null;
  } else {
    const messagesNumber = voteMonitoringData[0].value;
    const messagesWithProblems = voteMonitoringData[5].value;
    const percent = (messagesWithProblems / messagesNumber) * 100;
    return (
      <div className={"full-width-container"}>
        <div className={"vote-monitoring-container"}>
          <div className={"vote-monitoring-title"}>
            <h1>OBSERVAREA INDEPENDENTĂ A ALEGERILOR</h1>
          </div>
          <div className="vote-monitoring-message">
            Aceste date sunt colectate prin aplicația Monitorizare Vot, dezvoltată de Code for Romania, de la observatorii independenți acreditați în secțiile de votare acoperite.
          </div>
          <div className={"vote-monitoring-numbers"}>
            {NumberArea({
              bigNumber: voteMonitoringData[1].value,
              text: "Secții de votare acoperite"
            })}
            {NumberArea({
              bigNumber: voteMonitoringData[2].value,
              text: "Județe acoperite"
            })}
            {NumberArea({
              bigNumber: voteMonitoringData[4].value,
              text: "Observatori logați în aplicație"
            })}
          </div>
          <div className={"vote-monitoring-info"}>
            <div className={"info-legend"}>
              <div className={"legend-container space"}>
                <div className={"square left"}></div>
                <p>Mesaje trimise de către observatori</p>
              </div>
              <div className={"legend-container"}>
                <div className={"square right"}></div>
                <p>Probleme sesizate</p>
              </div>
            </div>
            <div className={"info-legend bars"}>
              <div className={"parent-bar"}>
                <p style={{ paddingLeft: `${percent + 9}%` }}>
                  {voteMonitoringData[0].value}
                </p>
              </div>
              <div
                style={{
                  width: `${percent}%`
                }}
                className={"child-bar"}
              >
                <p>{voteMonitoringData[5].value}</p>
              </div>
            </div>
          </div>
        </div>
        <ElectionChart />
      </div>
    );
  }
};
