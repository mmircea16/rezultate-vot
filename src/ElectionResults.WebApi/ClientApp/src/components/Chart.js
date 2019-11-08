import React from "react";
import "./Chart.css";
import * as signalR from "@aspnet/signalr";

const Line = ({ percent }) => (
  <div className="chart-line" style={{ top: `calc(100% - ${percent}%)` }}>
    <div className="chart-line-text">{`${percent}%`}</div>
  </div>
);

const Legend = ({ percent, count, text }) => (
  <div className="legend">
    {percent ? <div className="percentage">{`${percent} %`}</div> : ""}
    <div className={percent ? "count" : "percentage"}>{count}</div>
    <div className="text">{text}</div>
  </div>
);

const StripedBar = ({ color, percent, count, text }) => (
  <div
    className="chart-bar"
    style={{
      background: `repeating-linear-gradient(-45deg, ${color} 0 10px, #fff 10px 12px)`,
      height: `${percent}%`
    }}
  >
    <Legend percent={percent} count={dotFormat(count)} text={text} />
  </div>
);

const dotFormat = value => `${value}`.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");

export function ElectionChart() {
  const [data, setData] = React.useState({});

  const calcPercentage = val => ((val * 100) / data.enlistedVoters).toFixed(2);

  React.useEffect(() => {
    fetch("/api/results/voter-turnout")
      .then(result => result.json())
          .then(result => setData(result));
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/live-results")
        .build();

    connection
        .start()
        .then(() => console.log("Connection started!"))
        .catch(err => console.log("Error while establishing connection :("));

    connection.on("turnout-updated", data => {
        console.log("received turnout data in chars");
        console.log(data);
        setData(data);
    });
  }, []);
  if (!data) {
    return null;
  } else {
    return (
      <div className="border-radius">
        <div className={"vote-monitoring-title-presence"}>
            <h1>PREZENȚĂ LA VOT</h1>
        </div>
        <div className="vote-monitoring-total">
          { dotFormat(data.enlistedVoters) } număr total de înscriși pe listele permanente și speciale
        </div>
        <div className="x-container">        
          <div>
            <div className="text-center chart-title">{ dotFormat(data.enlistedVoters) } votanți</div>
            <div className="chart">
              <div>
                <Line percent={0} />
                <Line percent={25} />
                <Line percent={50} />
                <Line percent={75} />
                <Line percent={100} />
              </div>
              <div className="chart-container">
                <StripedBar
                  color="#FFCC00"
                  percent={data.turnoutPercentage}
                  count={data.totalNationalVotes}
                  text="Prezența la urne"
                />
                <StripedBar
                  color="#3C8CD2"
                  percent={calcPercentage(data.permanentLists)}
                  count={data.permanentLists}
                  text="Votanți pe liste permanente și speciale"
                />
                <StripedBar
                  color="#443F46"
                  percent={calcPercentage(data.additionalLists)}
                  count={data.additionalLists}
                  text="Votanți pe liste suplimentare"
                />
                <StripedBar
                  color="#F74B32"
                  percent={calcPercentage(data.mobileVotes)}
                  count={data.mobileVotes}
                  text="Votanți cu urnă mobilă"
                />
              </div>
            </div>
          </div>
          <div>
            <div className="text-center chart-title">Diaspora</div>
            <div className="chart" style={{ justifyContent: "center" }}>
              <div
                className="chart-bar"
                style={{
                  background: "#FFCC00",
                  height: "45%",
                  alignSelf: "flex-end"
                }}
              >
                <Legend
                                count={dotFormat(data.totalDiasporaVotes)}
                                text={`Alegători din care ${dotFormat(data.diasporaWithoutMailVotes)} prezenți la urne`}
                />
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
