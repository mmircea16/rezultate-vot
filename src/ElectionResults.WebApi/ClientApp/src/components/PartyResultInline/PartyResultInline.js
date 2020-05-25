import React from "react";
import "./PartyResultInline.css"

const PartyResultInline = ({result}) => {
    const party = result.entity;
    return <div className={"party-result-inline"}>
        <div className={"party-name"}><div className={"color-marker"} style={{backgroundColor: party.color}}/>{party.name}</div>
        <div className={"votes"}>{result.percentage}% ({result.votes})</div>
    </div>
};

export default PartyResultInline;