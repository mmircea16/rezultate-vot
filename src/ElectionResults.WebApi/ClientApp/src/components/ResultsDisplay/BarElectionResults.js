import React from "react"
import PropTypes from 'prop-types';
import {Election} from "../../domain/Election"

export const BarElectionResults = ({electionResults}) => {
    return <div>
        <div>Prezenta vot: {electionResults.turnout}</div>
    </div>
};

BarElectionResults.propTypes = {
    electionResults: PropTypes.instanceOf(Election)
};