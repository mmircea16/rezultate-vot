import React from "react";
import {HorizontalStackedBar} from "./HorizontalStackedBar";
import {number, withKnobs} from "@storybook/addon-knobs";
import {Result} from "../../domain/Result";
import {Party} from "../../domain/Party";

export default {
    title: "Horizontal Stacked Bar",
    component: HorizontalStackedBar,
    decorators: [withKnobs]
}

export const SimpleExample = () => {

    const results = [
        new Result(Party.PSD, number("PSD votes", 2000)),
        new Result(Party.UDMR, number("UDMR votes", 700)),
        new Result(Party.PMP, number("PMP votes", 500)),
        new Result(Party.USR, number("USR votes", 1000)),
        new Result(Party.PNL, number("PNL votes", 1800))
    ];

    return <HorizontalStackedBar results={results}/>
};