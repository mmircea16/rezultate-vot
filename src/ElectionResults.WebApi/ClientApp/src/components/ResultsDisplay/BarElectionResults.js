import React from "react"
import PropTypes from 'prop-types';
import {Election} from "../../domain/Election"
import {HorizontalStackedBar} from "../Charts/HorizontalStackedBar";
import PartyResultCard from "../PartyResultCard/PartyResultCard";
import "./BarElectionResults.css"
import {Result} from "../../domain/Result";

export const BarElectionResults = ({electionResults}) => {
    const orderedResults = alternate(electionResults.results);
    return <div>
        <div className={"result-card-container"}>
            {orderedResults.map(result => <PartyResultCard key={result.entity.name} result={result}/>)}
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
        .map(result => new Result(result.entity, Math.abs(result.votes), result.percentage));
};

BarElectionResults.propTypes = {
    electionResults: PropTypes.instanceOf(Election)
};