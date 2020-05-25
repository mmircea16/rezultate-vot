import React from "react";
import "./PartyResultInline.css"
import NameWithColor from "../NameWithColor/NameWithColor";

const PartyResultInline = ({result}) => {
    const party = result.entity;
    return <div className={"party-result-inline"}>
        <NameWithColor color={party.color} text={party.name}/>
        <div className={"votes"}>{result.percentage}% ({result.votes})</div>
    </div>
};

export default PartyResultInline;