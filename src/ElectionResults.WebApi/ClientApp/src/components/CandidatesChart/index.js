import React from "react";
import { ChartBar } from "./ChartBar";
import CountiesSelect from "./CountiesSelect";
import { FormGroup, Col, Button } from "reactstrap";
let currentSelection = '';

export const ChartContainer = () => {
    const [showAll, toggleShowAll] = React.useState(false);
    const [candidates, setCandidates] = React.useState(null);
    const [counties, setCounties] = React.useState(null);
    const [voterTurnout, setVoterTurnout] = React.useState(null);
    const [displayQuestion, setDisplayQuestion] = React.useState(false);
    React.useEffect(() => {
        const fetchServerData = async () => {
            try {
                console.log("candidates fetch");
                fetch("/api/results/voter-turnout")
                    .then(data => data.json())
                    .then(data => setVoterTurnout(data));
                return;
                fetch(`/api/results?location=${currentSelection}`)
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
            } catch (e) {
                console.log(e);
            }
        };

        const onIdle = () => {
            clearInterval(timer);
            timer = null;
        }
        const onActive = () => {
            timer = setInterval(() => fetchServerData(), 1000 * 30);
        }
        window.addEventListener("onIdle", onIdle);
        window.addEventListener("onActive", onActive);
        fetchServerData();
        let timer = setInterval(() => fetchServerData(), 1000 * 30);
        return () => {
            console.log("cleaned up");
            clearInterval(timer);
            timer = null;
            window.removeEventListener()("onIdle", onIdle);
            window.removeEventListener()("onActive", onActive);
        };
    }, []);

    const selectionChanged = value => {
        currentSelection = value.id;
        fetch(`/api/results?location=${currentSelection}`)
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
