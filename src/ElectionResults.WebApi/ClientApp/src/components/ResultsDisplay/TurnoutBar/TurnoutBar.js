import React from "react";
import PropTypes from "prop-types";
import {Election} from "../../../domain/Election";
import PercentageAsBar from "../../Charts/PercentageAsBar/PercentageAsBar";
import NameWithColor from "../../NameWithColor/NameWithColor";
import "./TurnoutBar.css"

const TurnoutBar = ({election}) => {
    const totalColor = "#FFC539";
    const turnoutColor = "#3e11a9";
    const turnoutPercentage = election.getTurnoutPercentage();

    return <div className={"turnout-bar"}>
        <div className={"title"}>Prezenta Vot</div>
        <PercentageAsBar value={turnoutPercentage} totalColor={totalColor} turnoutColor={turnoutColor}/>
        <NameWithColor text={`Cetateni cu drept de vot 100% (${election.total})`} color={totalColor}/>
        <NameWithColor text={`Au votat ${turnoutPercentage}% (${election.turnout})`} color={turnoutColor}/>
    </div>
};

export default TurnoutBar;

TurnoutBar.propTypes = {
    election: PropTypes.instanceOf(Election).isRequired,
};