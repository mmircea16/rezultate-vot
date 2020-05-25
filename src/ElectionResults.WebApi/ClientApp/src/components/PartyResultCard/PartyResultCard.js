import React from "react";
import PropTypes from "prop-types"
import {Result} from "../../domain/Result";
import "./PartyResultCard.css"
import NameWithColor from "../NameWithColor/NameWithColor";

const PartyResultCard = ({result}) => {
    const party = result.entity;
    return <div className={"party-result-card"}>
        <NameWithColor color={party.color} text={party.name}/>
        <div className={"percentage"}>{result.percentage}%</div>
    </div>
};

export default PartyResultCard;

PartyResultCard.propTypes = {
    result: PropTypes.instanceOf(Result).isRequired
};