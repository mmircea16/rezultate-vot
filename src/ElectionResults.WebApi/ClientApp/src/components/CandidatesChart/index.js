import React from "react";
import { ChartBar } from "./ChartBar";
import CountiesSelect from "./CountiesSelect";
import { FormGroup, Col, Button } from "reactstrap";

const candidates = [
  {
    id: "1",
    name: "Candidate X",
    votes: 1209484,
    imageUrl:
      "https://us.123rf.com/450wm/kritchanut/kritchanut1406/kritchanut140600112/29213222-stock-vector-male-silhouette-avatar-profile-picture.jpg?ver=6&fbclid=IwAR0HzpXjYDnMwHFfD8fdnNFrBa8rmLJ74i3NbOsPZvzwNNY7WANjuX6DvGA",
    percentage: 60
  },
  {
    id: "2",
    name: "Candidate X",
    votes: 1209484,
    imageUrl: "",
    percentage: 25
  },
  {
    id: "3",
    name: "Candidate X",
    votes: 1209484,
    imageUrl: "",
    percentage: 40
  },
  {
    id: "4",
    name: "Candidate X",
    votes: 1209484,
    imageUrl: "",
    percentage: 60
  },
  {
    id: "5",
    name: "Candidate X",
    votes: 1209484,
    imageUrl: "",
    percentage: 25
  },
  {
    id: "6",
    name: "Candidate X",
    votes: 1209484,
    imageUrl: "",
    percentage: 40
  },
  {
    id: "7",
    name: "Candidate X",
    votes: 1209484,
    imageUrl: "",
    percentage: 60
  },
  {
    id: "8",
    name: "Candidate X",
    votes: 1209484,
    imageUrl: "",
    percentage: 25
  },
  {
    id: "9",
    name: "Candidate X",
    votes: 1209484,
    imageUrl: "",
    percentage: 40
  }
];

export const ChartContainer = () => {
  const [showAll, toggleShowAll] = React.useState(false);
  // const [candidates, setCandidates] = React.useState(null);
  React.useEffect(() => {
    // fetch(
    //   "insert url here"
    // )
    //   .then(data => data.json())
    //   .then(data => {
    //     setCandidates(data.candidates);
    //   });
  });
  let changeSelection = value => console.log(value);
  return (
    <div>
      {candidates ? (
        <div>
          <div sm={12} className={"votes-container"}>
            <p className={"container-text"}>
              Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
              eiusmod tempor incididunt ut labore et dolore magna aliqua.
              Viverra nam libero justo laoreet sit amet cursus sit. Malesuada
              nunc vel risus commodo viverra maecenas.
            </p>
            <div sm={3} className={"votes-numbers"}>
              <h3 className={"votes-title"}>Voturi numarate</h3>
              <div sm={3} className={"votes-results"}>
                <p className={"votes-percent"}>30%</p>
                <p className={"votes-text"}>12.400.000</p>
              </div>
            </div>
          </div>
          <FormGroup row>
            <Col sm={3}>
              <CountiesSelect onSelection={changeSelection} />
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
              Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
              eiusmod tempor incididunt ut labore et dolore magna aliqua.
              Viverra nam libero justo laoreet sit amet cursus sit. Malesuada
              nunc vel risus commodo viverra maecenas.
            </p>
          </div>
          <div className={"question"}>
            <p className={"question-text"}>
              Cine sunt candidatii care merg in turul 2?
            </p>
          </div>
        </div>
      )}
    </div>
  );
};
