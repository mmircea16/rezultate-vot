import React from "react";
import TableResults from "./TableResults";
import {number, withKnobs} from "@storybook/addon-knobs";
import Calc from "../../../utils/Calc";
import {Result} from "../../../domain/Result";
import {Party} from "../../../domain/Party";
import {Election} from "../../../domain/Election";

export default {
    title: "Table Results",
    component: TableResults,
    decorators: [withKnobs]
}

export  const SimpleExample = () => {
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

    const election = new Election("Alegeri Parlamentare", results, turnout, 15000, "Senat 2016");
    return <TableResults election={election}/>
};