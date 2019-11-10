import React from "react";
import "./style.css";
import { ElectionChart } from "../Chart";

const NumberArea = ({ bigNumber, text }) => (
    <div className={"vote-monitoring-area"}>
        <p className={"area-number"}>{bigNumber}</p>
        <p className={"area-text"}>{text}</p>
    </div>
);
export const VoteMonitoring = () => {
    const [voteMonitoringData, setVoteMonitoringData] = React.useState(null);
    React.useEffect(() => {
        const fetchServerData = async () => {
            try {
                console.log("monitoring data");
                fetch("/api/results/monitoring")
                    .then(data => data.json())
                    .then(data => setVoteMonitoringData(data.statistics));
            } catch (e) {
                console.log(e);
            }
        };

        const onIdle = () => {
            clearInterval(timer);
            timer = null;
        }
        const onActive = () => {
            timer = setInterval(() => fetchServerData(), 1000 * 60 * 10);
        }
        window.addEventListener("onIdle", onIdle);
        window.addEventListener("onActive", onActive);
        fetchServerData();
        let timer = setInterval(() => fetchServerData(), 1000 * 60 * 10);
        return () => {
            console.log("cleaned up vote monitoring component");
            clearInterval(timer);
            timer = null;
            window.removeEventListener()("onIdle", onIdle);
            window.removeEventListener()("onActive", onActive);
        };
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
                            // bigNumber: voteMonitoringData[2].value,
                            bigNumber: 1,
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
