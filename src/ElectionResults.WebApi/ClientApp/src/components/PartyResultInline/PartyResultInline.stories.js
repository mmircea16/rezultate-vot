import React from "react";
import PartyResultInline from "./PartyResultInline";
import {Result} from "../../domain/Result";
import {Party} from "../../domain/Party";

export default {
    title: "Party Result Inline",
    component: PartyResultInline,
}



export const SimpleExample = () => {
    const result = new Result(Party.PSD, 220000, 32.7);
    return <PartyResultInline result={result}/>
};