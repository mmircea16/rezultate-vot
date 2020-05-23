import React from "react"
import PropTypes from 'prop-types';
import {Election} from "../../domain/Election"
import {HorizontalStackedBar} from "../Charts/HorizontalStackedBar";

export const BarElectionResults = ({electionResults}) => {
    return <div>
        <div>Prezenta vot: {electionResults.turnout}</div>
        <HorizontalStackedBar results={electionResults.results} orderType={"ALTERNATE"} width={700} height={100}/>
    </div>
};

BarElectionResults.propTypes = {
    electionResults: PropTypes.instanceOf(Election)
};