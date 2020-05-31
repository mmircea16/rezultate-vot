import React, {useState} from "react";
import {BarElectionResults} from "../ResultsDisplay/BarElectionResults/BarElectionResults";
import {HistoricResultsService} from "../../services/HistoricResultsService";
import TableResults from "../ResultsDisplay/TableResults/TableResults";

export const CountyCouncilResults = () => {
    const [county, setCounty] = useState();
    const [election, setElection] = useState();

    const getElectionResults = (county) => HistoricResultsService.getResults("Consiliu Judetean", "Nov-19", county);
    const displayResults = function () {
        return <div>
            {select()}
            <BarElectionResults election={election} showFirst={5}/>
            <TableResults election={election}/>
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
                {counties.map(county => <option value={county}>{county}</option> )}
            </select>
        </div>;
    };

    if (county) {
        return displayResults()
    } else {
        return select()
    }

};