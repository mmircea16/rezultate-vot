import React from "react";
import PropTypes from "prop-types"
import {Result} from "../../domain/Result";
import "./PartyResultCard.css"

const PartyResultCard = ({result}) => {
    const party = result.entity;
    return <div className={"party-result-card"}>
        <div className={"party-name"}><div className={"color-marker"} style={{backgroundColor: party.color}}/>{party.name}</div>
        <div className={"percentage"}>{result.percentage}%</div>
    </div>
};

export default PartyResultCard;

PartyResultCard.propTypes = {
    result: PropTypes.instanceOf(Result).isRequired
};