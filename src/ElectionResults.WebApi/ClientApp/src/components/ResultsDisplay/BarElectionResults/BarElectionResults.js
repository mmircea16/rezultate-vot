import React from "react"
import PropTypes from 'prop-types';
import {Election} from "../../../domain/Election"
import {HorizontalStackedBar} from "../../Charts/HorizontalStackedBar/HorizontalStackedBar";
import PartyResultCard from "../../PartyResultCard/PartyResultCard";
import "./BarElectionResults.css"
import {Result} from "../../../domain/Result";
import PartyResultInline from "../../PartyResultInline/PartyResultInline";
import Calc from "../../../utils/Calc";

export const BarElectionResults = ({election}) => {
    const orderedResults = alternate(election.results);
    const descSortedResults = election.results.sort((a, b) => b.votes - a.votes);
    const turnoutPercentage = Calc.percentageTo2Decimals(election.turnout, election.total);

    return <div>
        <div className={"results-card-container inline-results"}>
            {orderedResults.map(result => <PartyResultCard key={result.entity.name} result={result}/>)}
        </div>
        <HorizontalStackedBar results={orderedResults}/>
        <div className={"turnout"}>Din {turnoutPercentage}% ({election.turnout}) voturi</div>
        <div className={"results-inline-container inline-results"}>
            {descSortedResults.map(result => <PartyResultInline key={result.entity.name} result={result}/> )}
        </div>
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
    election: PropTypes.instanceOf(Election)
};