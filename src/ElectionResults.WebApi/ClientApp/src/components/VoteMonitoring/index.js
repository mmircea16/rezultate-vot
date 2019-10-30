import React from "react";
import "./style.css";

const NumberArea = ({ bigNumber, text }) => (
  <div className={"vote-monitoring-area"}>
    <p className={"area-number"}>{bigNumber}</p>
    <p className={"area-text"}>{text}</p>
  </div>
);
export const VoteMonitoring = () => {
  return (
    <div>
      <div className={"vote-monitoring-container"}>
        <div className={"vote-monitoring-title"}>
          <h1>OBSERVAREA INDEPENDENTĂ A ALEGERILOR</h1>
        </div>
        <div className={"vote-monitoring-numbers"}>
          {NumberArea({ bigNumber: 324, text: "Secții de votare" })}
          {NumberArea({ bigNumber: 324, text: "Secții de votare" })}
          {NumberArea({ bigNumber: 324, text: "Secții de votare" })}
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
                color: "#fff"
              }}
            >
              <p>500</p>
            </div>
            <div
              style={{
                width: "60%",
                backgroundColor: "#EA8C42",
                height: "56px",
                position: "absolute",
                top: "29px",
                left: "0",
                color: "#fff"
              }}
            >
              <p>5049</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
