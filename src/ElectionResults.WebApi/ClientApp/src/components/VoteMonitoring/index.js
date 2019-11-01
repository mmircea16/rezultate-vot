import React from "react";
import "./style.css";

const NumberArea = ({ bigNumber, text }) => (
  <div className={"vote-monitoring-area"}>
    <p className={"area-number"}>{bigNumber}</p>
    <p className={"area-text"}>{text}</p>
  </div>
);
export const VoteMonitoring = ({ voteMonitoringData }) => {
  console.log(percent);
  let numberFull;
  let numberSmall;
  let percent;
  if (!voteMonitoringData) {
    return null;
  } else {
    const numberFull = voteMonitoringData[0].value;
    const numberSmall = voteMonitoringData[5].value;
    const percent = (numberSmall / numberFull) * 100;
    return (
      <div className={"full-width-container"}>
        <div className={"vote-monitoring-container"}>
          <div className={"vote-monitoring-title"}>
            <h1>OBSERVAREA INDEPENDENTĂ A ALEGERILOR</h1>
          </div>
          <div className={"vote-monitoring-numbers"}>
            {NumberArea({
              bigNumber: voteMonitoringData[1].value,
              text: "Secții de votare"
            })}
            {NumberArea({
              bigNumber: voteMonitoringData[2].value,
              text: "Județe acoperite"
            })}
            {NumberArea({
              bigNumber: voteMonitoringData[4].value,
              text: "Observatori în secții"
            })}
          </div>
          <div className={"vote-monitoring-info"}>
            <div className={"info-legend"}>
              <div
                style={{
                  display: "flex",
                  marginRight: "56px",
                  alignItems: "center"
                }}
              >
                <div
                  style={{
                    height: "20px",
                    width: "20px",
                    backgroundColor: "#110D64",
                    fontSize: "20px",
                    marginRight: "10px"
                  }}
                ></div>
                <p>Mesaje trimise de către observatori</p>
              </div>
              <div style={{ display: "flex", alignItems: "center" }}>
                <div
                  style={{
                    height: "20px",
                    width: "20px",
                    backgroundColor: "#EA8C42",
                    fontSize: "20px",
                    marginRight: "10px"
                  }}
                ></div>
                <p>Probleme sesizate</p>
              </div>
            </div>
            <div className={"info-legend bars"}>
              <div
                style={{
                  width: "100%",
                  height: "56px",
                  backgroundColor: "#110D64",
                  color: "#fff",
                  display: "flex",
                  alignItems: "center",
                  fontSize: "1.5em"
                }}
              >
                <p style={{ marginLeft: `${percent + 3}%` }}>
                  {voteMonitoringData[0].value}
                </p>
              </div>
              <div
                style={{
                  width: `${percent}%`,
                  backgroundColor: "#EA8C42",
                  height: "56px",
                  position: "absolute",
                  top: "29px",
                  left: "0",
                  color: "#fff",
                  display: "flex",
                  alignItems: "center",
                  padding: "5px",
                  fontSize: "1.5em"
                }}
              >
                <p>{voteMonitoringData[5].value}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
};
