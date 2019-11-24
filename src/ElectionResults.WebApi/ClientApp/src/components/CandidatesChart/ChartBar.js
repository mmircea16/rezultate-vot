import React from "react";
import "./styles.css";

export const ChartBar = ({ percent, displayPercentage, imgUrl, candidateName, votesNumber }) => {
    return (
        <div className={"bar-container"}>
            <div className={"bar-candidate"}>
                <p className={"bar-candidate-name"}>{candidateName}</p>
            </div>
            <div className={"bar-icon"}>
                <img src={imgUrl} alt="" />
            </div>

            <div className={"bar-results"}>
                <div className={"bar-votes"} style={{ width: `${displayPercentage}%` }}></div>
                <div className={"bar-votes-text"}>
                    <p className={"bar-votes-percent"}>{`${percent}%`}</p>
                </div>

                <div className={"bar-votes-result"}>
                    <p className={"bar-votes-number"}>{votesNumber.toLocaleString(undefined, { maximumFractionDigits: 2 })}</p>
                </div>
            </div>
        </div>
    );
};
