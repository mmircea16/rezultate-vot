import React from "react"
import PropTypes from 'prop-types';
import {Election} from "../../../domain/Election"
import {HorizontalStackedBar} from "../../Charts/HorizontalStackedBar/HorizontalStackedBar";
import PartyResultCard from "../../PartyResultCard/PartyResultCard";
import "./BarElectionResults.css"
import {Result} from "../../../domain/Result";
import PartyResultInline from "../../PartyResultInline/PartyResultInline";
import {Party} from "../../../domain/Party";

export const BarElectionResults = ({election, showFirst}) => {
    let results = election.results;

    if (showFirst && showFirst < results.length - 1) {
        const sortedResults = sortDesc(results);
        const others = sortedResults.slice(showFirst);
        const {othersVotes, othersPercentage} = others.reduce(
            ({othersVotes, othersPercentage}, result) =>
                ({othersVotes: othersVotes + result.votes, othersPercentage: othersPercentage + result.percentage}),
            {othersVotes: 0, othersPercentage: 0}
        );
        results = sortedResults.slice(0, showFirst);
        results.push(new Result(Party.OTHERS, othersVotes, othersPercentage))
    }

    const orderedResults = alternate(results);
    const descSortedResults = sortDesc(results);
    const turnoutPercentage = election.getTurnoutPercentage();
    const half = Math.floor(results.length / 2);

    return <div className={"bar-election-results"}>
        <div className={"title"}>Rezultate Vot</div>
        <div className={"results-card-container inline-results"}>
            {orderedResults.map((result, index) => <PartyResultCard key={result.entity.name} result={result} rightAligned={index > half}/>)}
        </div>
        <HorizontalStackedBar results={orderedResults}/>
        <div className={"turnout"}>Din {turnoutPercentage}% ({election.turnout}) voturi</div>
        <div className={"results-inline-container inline-results"}>
            {descSortedResults.map(result => <PartyResultInline key={result.entity.name} result={result}/>)}
        </div>
    </div>
};

const sortDesc = function (results) {
    return results.sort((a, b) => b.votes - a.votes);
};

const alternate = function (results) {
    let rearrangedResults = results
        .sort((a, b) => b.votes - a.votes)
        .map((result, index) => ({...result, votes: index % 2 ? result.votes : (-1) * result.votes}));

    return rearrangedResults
        .sort((a, b) => a.votes - b.votes)
        .map(result => new Result(result.entity, Math.abs(result.votes), result.percentage));
};

BarElectionResults.propTypes = {
    election: PropTypes.instanceOf(Election).isRequired,
    showFirst: PropTypes.number
};