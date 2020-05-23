import React from "react"
import PropTypes from 'prop-types';
import {Election} from "../../domain/Election"
import {HorizontalStackedBar} from "../Charts/HorizontalStackedBar";
import PartyResultCard from "../PartyResultCard/PartyResultCard";
import "./BarElectionResults.css"

export const BarElectionResults = ({electionResults}) => {
    const orderedResults = alternate(electionResults.results);
    return <div>
        <div className={"result-card-container"}>
            {orderedResults.map(result => <PartyResultCard result={result}/>)}
        </div>
        <HorizontalStackedBar results={orderedResults}/>
    </div>
};

const alternate = function(results) {
    let rearrangedResults = results
        .sort((a, b) => b.votes - a.votes)
        .map((result, index) => ({...result, votes: index % 2 ? result.votes : (-1)*result.votes}));

    return rearrangedResults
        .sort((a, b) => a.votes - b.votes)
        .map(result => ({...result, votes: Math.abs(result.votes)}));
};

BarElectionResults.propTypes = {
    electionResults: PropTypes.instanceOf(Election)
};