import React from "react";
import {BarElectionResults} from "./BarElectionResults";
import {number, withKnobs} from "@storybook/addon-knobs";
import {Result} from "../../../domain/Result";
import {Party} from "../../../domain/Party";
import {Election} from "../../../domain/Election";
import Calc from "../../../utils/Calc";

export default {
    title: "Bar Election Results",
    component: BarElectionResults,
    decorators: [withKnobs]
}

export const SimpleExample = () => {

    const pnlVotes = number("PNL votes", 1800);
    const pmpVotes = number("PMP votes", 500);
    const psdVotes = number("PSD votes", 2000);
    const udmrVotes = number("UDMR votes", 700);
    const usrVotes = number("USR votes", 1200);

    const turnout = pnlVotes + pmpVotes + psdVotes + udmrVotes + usrVotes;

    const percentage = votes => Calc.percentageTo2Decimals(votes, turnout);

    const results = [
        new Result(Party.PNL, pnlVotes, percentage(pnlVotes)),
        new Result(Party.PMP, pmpVotes, percentage(pmpVotes)),
        new Result(Party.PSD, psdVotes, percentage(psdVotes)),
        new Result(Party.UDMR, udmrVotes, percentage(udmrVotes)),
        new Result(Party.USR, usrVotes, percentage(usrVotes)),
    ];

    const election = new Election("council", results, turnout, 15000);

    return <BarElectionResults election={election}/>
};