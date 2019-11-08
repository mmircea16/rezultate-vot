import React from "react";
import "./Chart.css";
import useWindowSize from '../hooks/useWindowSize';
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

const StripedBar = ({ color, percent, count, text, orizontal }) => {
  const style = { background: `repeating-linear-gradient(-45deg, ${color} 0 10px, #fff 10px 12px)` }
  if (orizontal) {
    style.width = `${percent}%`;
    style.height = '48px';
  } else {
    style.height = `${percent}%`;
  }
  return (
    <div className="chart-bar" style={style}>
      <Legend percent={percent} count={dotFormat(count)} text={text} />
    </div>
  )
}

const LegendDot = ({ color, text }) => (
  <div style={{ display: 'flex', margin: '5px 0' }}>
    <div style={{
      width: '20px',
      height: '20px',
      display: 'inline-block',
      background: `repeating-linear-gradient(-45deg, ${color} 0 10px, #fff 10px 12px)`,
    }}></div>
    <span style={{ paddingLeft: '10px' }}>{text}</span>
  </div>
)

const dotFormat = value => `${value}`.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");

export function ElectionChart() {
  const MAX_MOBILE_WIDTH = 575;
  const windowSize = useWindowSize();
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
        {windowSize.width > MAX_MOBILE_WIDTH
          ? (
            <div className="vote-monitoring-total">
              {dotFormat(data.enlistedVoters)} număr total de înscriși pe listele permanente și speciale
            </div>)
          : null
        }
        <div className="x-container">
          <div>
            {windowSize.width > MAX_MOBILE_WIDTH
              ? <div className="text-center chart-title">Național</div>
              : (
                <div>
                  <h1>PREZENȚĂ LA VOT</h1>
                  <div>{dotFormat(data.enlistedVoters) + " înscriși pe liste permanente și speciale"}</div>
                </div>)
            }
            <div className="chart">
              {windowSize.width > MAX_MOBILE_WIDTH
                ? (
                  <div>
                    <Line percent={0} />
                    <Line percent={25} />
                    <Line percent={50} />
                    <Line percent={75} />
                    <Line percent={100} />
                  </div>)
                : null
              }
              <div className="chart-container">
                <StripedBar
                  orizontal={windowSize.width <= MAX_MOBILE_WIDTH}
                  color="#FFCC00"
                  percent={data.turnoutPercentage}
                  count={data.totalNationalVotes}
                  text="Prezența la urne"
                />
                <StripedBar
                  orizontal={windowSize.width <= MAX_MOBILE_WIDTH}
                  color="#3C8CD2"
                  percent={calcPercentage(data.permanentLists)}
                  count={data.permanentLists}
                  text="Votanți pe liste permanente și speciale"
                />
                <StripedBar
                  orizontal={windowSize.width <= MAX_MOBILE_WIDTH}
                  color="#443F46"
                  percent={calcPercentage(data.additionalLists)}
                  count={data.additionalLists}
                  text="Votanți pe liste suplimentare"
                />
                <StripedBar
                  orizontal={windowSize.width <= MAX_MOBILE_WIDTH}
                  color="#F74B32"
                  percent={calcPercentage(data.mobileVotes)}
                  count={data.mobileVotes}
                  text="Votanți cu urnă mobilă"
                />
              </div>
            </div>
          </div>
          <div>
            {windowSize.width > MAX_MOBILE_WIDTH
              ? <div className="text-center chart-title">Diaspora</div>
              : (<h1>Diaspora</h1>)
            }
            <div className="chart diaspora-chart">
              <div
                className="chart-bar"
                style={{
                  background: "#FFCC00",
                  height: windowSize.width > MAX_MOBILE_WIDTH ? "45%" : "48px",
                  alignSelf: "flex-end"
                }}
              >
                <Legend
                  count={windowSize.width > MAX_MOBILE_WIDTH
                    ? dotFormat(data.totalDiasporaVotes)
                    : dotFormat(data.diasporaWithoutMailVotes)}
                  text={windowSize.width > MAX_MOBILE_WIDTH
                    ? `Alegători din care ${dotFormat(data.diasporaWithoutMailVotes)} prezenți la urne`
                    : 'Alegători prezenți la urne'}
                />
              </div>
            </div>
          </div>
          {windowSize.width <= MAX_MOBILE_WIDTH
            ? (
              <>
                <hr />
                <div>
                  <h2>LEGENDĂ</h2>
                  <LegendDot color="#FFCC00" text="Prezența la urne" />
                  <LegendDot color="#3C8CD2" text="Votanți pe liste permanente și speciale" />
                  <LegendDot color="#443F46" text="Votanți pe liste suplimentare" />
                  <LegendDot color="#F74B32" text="Votanți cu urnă mobilă" />
                </div>
              </>)
            : null
          }
        </div>
      </div>
    );
  }
}
