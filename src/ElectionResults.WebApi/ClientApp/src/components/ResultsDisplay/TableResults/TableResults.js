import React from "react";
import PropTypes from "prop-types";
import {Election} from "../../../domain/Election";
import NameWithColor from "../../NameWithColor/NameWithColor";
import "./TableResults.css"

const TableResults = ({election}) => {
    const results = election.results.sort((a, b) => b.votes - a.votes);
    const MINI_BAR_MAX_WIDTH_IN_PX = 80;
    return <div className={"table-results"}>
        <div className={"election-type"}>{election.type}</div>
        <div className={"election-name"}>{election.name}</div>
        <table>
            <thead>
            <tr>
                <th>Partid</th>
                <th>Nr. voturi</th>
                <th>%</th>
            </tr>
            </thead>
            <tbody>
            {results.map(result => (
                <tr key={result.entity.name}>
                    <td><NameWithColor text={result.entity.name} color={result.entity.color}/></td>
                    <td>{result.votes}</td>
                    <td className={"shrink-to-size"}>{result.percentage}</td>
                    <td className={"shrink-to-size"}>
                        <div className={"mini-bar"}
                             style={{
                                 backgroundColor: result.entity.color,
                                 width: scaleInPixels(result.percentage, MINI_BAR_MAX_WIDTH_IN_PX)
                             }}/>
                    </td>
                </tr>
            ))}
            </tbody>
        </table>
    </div>
};

const scaleInPixels = (percentage, total) => `${Math.floor(total * percentage / 100)}px`;

export default TableResults;

TableResults.propTypes = {
    election: PropTypes.instanceOf(Election).isRequired,
    showFirst: PropTypes.number
};