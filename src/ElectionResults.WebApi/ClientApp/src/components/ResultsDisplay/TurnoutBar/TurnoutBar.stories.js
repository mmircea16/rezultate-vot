import React from "react";
import TurnoutBar from "./TurnoutBar";
import {number, withKnobs} from "@storybook/addon-knobs";
import {Election} from "../../../domain/Election";

export default {
    title: "Turnout Bar",
    component: TurnoutBar,
    decorators: [withKnobs]
}

export const SimpleExample = () => {
    const election = new Election("Alegeri Parlamentare", [], number("Turnout", 60000), 100000, "Senat 2016");
    return <TurnoutBar election={election}/>
};