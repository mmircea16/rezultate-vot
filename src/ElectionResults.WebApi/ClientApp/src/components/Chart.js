import React from 'react';
import './Chart.css';

const Line = ({ percent }) => (
  <div className="chart-line" style={{ top: `calc(100% - ${percent}%)` }}>
    <div className="chart-line-text">{`${percent}%`}</div>
  </div>
)

const Legend = ({ percent, count, text }) => (
  <div className="legend">
    {percent ? <div className="percentage">{`${percent} %`}</div> : ''}
    <div className={percent ? "count" : "percentage"}>{count}</div>
    <div className="text">{text}</div>
  </div>
)

const StripedBar = ({ color, percent, count, text }) => (
  <div className="chart-bar" style={{ background: `repeating-linear-gradient(-45deg, ${color} 0 10px, #fff 10px 12px)`, height: `${percent}%` }}>
    <Legend percent={percent} count={dotFormat(count)} text={text} />
  </div>
)

const dotFormat = (value) => `${value}`.replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');

export function ElectionChart() {
  const [data, setData] = React.useState({});

  const calcPercentage = (val) => (val * 100 / data.enlistedVoters).toFixed(2);

  React.useEffect(() => {
    fetch('api/results/voter-turnout')
      .then(result => result.json())
      .then(result => setData(result))
  }, []);

  return (
    <div className="x-container">
      <div>
        <div className="text-center chart-title">Național</div>
        <div className="chart">
          <div>
            <Line percent={0} />
            <Line percent={25} />
            <Line percent={50} />
            <Line percent={75} />
            <Line percent={100} />
          </div>
          <div className="chart-container">
            <div className="chart-bar">
              <div className="legend" style={{ left: '-100%', top: '0' }}>
                <div className="percentage">100 %</div>
                <div className="count">{dotFormat(data.enlistedVoters)}</div>
                <div className="text">Înscriși pe liste permanente și speciale</div>
              </div>
            </div>
            <StripedBar color="#EA8C42" percent={data.turnoutPercentage} count={data.totalNationalVotes} text="Prezența la urne" />
            <StripedBar color="#3C8CD2" percent={calcPercentage(data.permanentLists)} count={data.permanentLists} text="Votanți pe liste permanente și speciale" />
            <StripedBar color="#443F46" percent={calcPercentage(data.additionalLists)} count={data.additionalLists} text="Votanți pe liste suplimentare" />
            <StripedBar color="#F74B32" percent={calcPercentage(data.mobileVotes)} count={data.mobileVotes} text="Votanți cu urnă mobilă" />
          </div>
        </div>
      </div>
      <div>
        <div className="text-center chart-title">Diaspora</div>
        <div className="chart" style={{ justifyContent: 'center' }}>
          <div className="chart-bar" style={{ background: '#EA8C42', height: '45%', alignSelf: 'flex-end' }}>
            <Legend count={dotFormat(data.totalDiasporaVotes)} text="Alegători prezenți la urne" />
          </div>
        </div>
      </div>
    </div>
  )
}
