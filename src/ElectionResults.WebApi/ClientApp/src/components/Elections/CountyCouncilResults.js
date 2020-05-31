import React, {useState} from "react";
import {BarElectionResults} from "../ResultsDisplay/BarElectionResults/BarElectionResults";
import {HistoricResultsService} from "../../services/HistoricResultsService";
import TableResults from "../ResultsDisplay/TableResults/TableResults";
import TurnoutBar from "../ResultsDisplay/TurnoutBar/TurnoutBar";
import "./CountyCouncilResults.css"

export const CountyCouncilResults = () => {
    const [county, setCounty] = useState();
    const [election, setElection] = useState();
    const [showResults, setShowResults] = useState(true);

    const getElectionResults = (county) => HistoricResultsService.getResults("Consiliu Judetean", "Nov-19", county);

    const results = () => {
        return <div>
            <BarElectionResults election={election} showFirst={5}/>
            <TableResults election={election}/>
        </div>
    };

    const turnout = () => {
        return <div>
            <TurnoutBar election={election}/>
        </div>
    };

    const displayResults = function () {
        return <div className={"results"}>
            {select()}
            <div className={"tabs"}>
                <div onClick={() => setShowResults(false)} className={showResults ? "":"selected"}>Prezenta Vot</div>
                <div onClick={() => setShowResults(true)} className={showResults ? "selected":""}>Rezultate Vot</div>
            </div>
            <div className={"container"}>
                {showResults && results()}
                {!showResults && turnout()}
            </div>
        </div>;
    };

    const countySelected = (event) => {
        const selectedCounty = event.target.value;
        setCounty(selectedCounty);
        setElection(getElectionResults(selectedCounty));
    };

    const counties = ["Prahova", "Buzau", "Sibiu", "Bucuresti"];

    const select = function () {
        return <div>
            <select onChange={countySelected} value={county}>
                <option disabled={!!county}>Alege judetul...</option>
                {counties.map(county => <option key={county} value={county}>{county}</option> )}
            </select>
        </div>;
    };

    if (county) {
        return displayResults()
    } else {
        return select()
    }

};