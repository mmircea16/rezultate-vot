import React from "react";
import { ChartBar } from "./ChartBar";
import CountiesSelect from "./CountiesSelect";
import { FormGroup, Col, Button } from "reactstrap";
import * as signalR from "@aspnet/signalr";

export const ChartContainer = () => {
  let timeInterval;
  const votingClosingTime = new Date(2019, 10, 10, 21);
  const [showAll, toggleShowAll] = React.useState(false);
  const [candidates, setCandidates] = React.useState(null);
  const [counties, setCounties] = React.useState(null);
  const [voterTurnout, setVoterTurnout] = React.useState(null);
  const [displayQuestion, setDisplayQuestion] = React.useState(new Date() >= votingClosingTime);

    React.useEffect(() => {
        if (!displayQuestion) {
          timeInterval = setInterval(() => {
            const currentTime = new Date();

            if (!displayQuestion && currentTime >= votingClosingTime) {
              setDisplayQuestion(true);
              clearInterval(timeInterval);
            }
          }, 1000);
        }

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/live-results")
            .build();

        connection
            .start()
            .then(() => console.log("Connection started!"))
            .catch(err => console.log("Error while establishing connection :("));

        connection.on("turnout-updated", data => {
            console.log("received turnout data in candidates");
            console.log(data);
            setVoterTurnout(data);
        });
        fetch("/api/results/voter-turnout")
            .then(data => data.json())
            .then(data => setVoterTurnout(data));

        return () => {
          if (timeInterval) {
            clearInterval(timeInterval);
          }
        };

        fetch("/api/results")
          .then(data => data.json())
          .then(data => {
            console.log(data);
            setCandidates(data.candidates);
            const total = { label: "Total", id: "TOTAL" };
            const national = { label: "National", id: "RO" };
            const diaspora = { label: "Diaspora", id: "DSPR" };
            data.counties.unshift(total, diaspora, national);
            setCounties(data.counties);
          });

        connection.on("results-updated", data => {
            console.log("received candidates");
            setCandidates(data.candidates);
        });
  }, []);

  const selectionChanged = value => {
    fetch(`/api/results?location=${value.id}`)
      .then(data => data.json())
      .then(data => setCandidates(data.candidates));
  };

  return (
    <div>
      {candidates ? (
        <div>
          <div sm={12} className={"votes-container"}>
            <p className={"container-text"}>
      Rezultate Vot, transparentizează întreg procesul electoral furnizând în timp real, într-o formă grafică intuitivă, ușor de înțeles, datele oficiale furnizate de către AEP și Birourile Electorale cât și datele preluate prin aplicația Monitorizare Vot dezvoltată de Code for Romania, despre alegerile din România.
      </p>
            <div sm={3} className={"votes-numbers"}>
              <h3 className={"votes-title"}>Voturi numarate</h3>
              <div sm={3} className={"votes-results"}>
                <p className={"votes-percent"}>
                  {" "}
                  {voterTurnout.turnoutPercentage}%
                </p>
                <p className={"votes-text"}>
                  {" "}
                  {voterTurnout.totalNationalVotes.toLocaleString(undefined, {
                    maximumFractionDigits: 2
                  })}
                </p>
              </div>
            </div>
          </div>
          <FormGroup row>
            <Col sm={3}>
              <CountiesSelect counties={counties} onSelect={selectionChanged} />
            </Col>
          </FormGroup>
          {(showAll ? candidates : candidates.slice(0, 5)).map(candidate => (
            <ChartBar
              key={candidate.id}
              percent={candidate.percentage}
              imgUrl={candidate.imageUrl}
              candidateName={candidate.name}
              votesNumber={candidate.votes}
            />
          ))}
          {!showAll ? (
            <div className={"show-all-container"} sm={3}>
              <Button className={"show-all-btn"} onClick={toggleShowAll}>
                Afiseaza toti candidatii
              </Button>
            </div>
          ) : null}
        </div>
      ) : (
        <div className={"default-container"}>
          <div className={"votes-container"}>
            <p className={"container-text"}>
      Rezultate Vot, transparentizează întreg procesul electoral furnizând în timp real, într-o formă grafică intuitivă, ușor de înțeles, datele oficiale furnizate de către AEP și Birourile Electorale cât și datele preluate prin aplicația Monitorizare Vot dezvoltată de Code for Romania, despre alegerile din România.
            </p>
          </div>
          {
            displayQuestion ? <div className={"question"}>
              <p className={"question-text"}>
                Cine sunt candidatii care merg in turul 2?
              </p>
            </div> : ""
          }
        </div>
      )}
    </div>
  );
};
