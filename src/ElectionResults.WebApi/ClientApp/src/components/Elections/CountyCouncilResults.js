import React from "react";
import {BarElectionResults} from "../ResultsDisplay/BarElectionResults/BarElectionResults";
import {HistoricResultsService} from "../../services/HistoricResultsService";

export const CountyCouncilResults = () => {
    let election = HistoricResultsService.getResults("council","Nov-19", "Prahova");
    return <div>County Council results:
        <div>
            <BarElectionResults electionResults={election}/>
        </div>
    </div>
};