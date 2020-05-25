import React from 'react';
import { shallow } from 'enzyme';
import {BarElectionResults} from "./BarElectionResults";
import {Result} from "../../../domain/Result";
import {Party} from "../../../domain/Party";
import {Election} from "../../../domain/Election";
import {HorizontalStackedBar} from "../../Charts/HorizontalStackedBar/HorizontalStackedBar";

describe(BarElectionResults, () => {
   it("should sort the results alternatively", () => {
      const results = [
         new Result(Party.PNL, 2800, 20),
         new Result(Party.PMP, 2000, 20),
         new Result(Party.PSD, 1800, 20),
         new Result(Party.UDMR, 1200, 20),
         new Result(Party.USR, 500, 20),
      ];

      //place the parties in desc order alternatively at the start and end of the bar - eg. first placed -> at the start
      //second placed -> at the end, third placed -> second from start, etc
      const expectedOrderResults = [
         new Result(Party.PNL, 2800, 20),
         new Result(Party.PSD, 1800, 20),
         new Result(Party.USR, 500, 20),
         new Result(Party.UDMR, 1200, 20),
         new Result(Party.PMP, 2000, 20),
      ];

      const election = new Election("council", results, 5000, 10000);
      const wrapper = shallow(<BarElectionResults election={election}/> );

      expect(wrapper.find(HorizontalStackedBar).props().results).toEqual(expectedOrderResults);
   });

   describe("with others", () => {
      const results = [
         new Result(Party.PNL, 2500, 50),
         new Result(Party.PSD, 2000, 40),
         new Result(Party.PMP, 250, 5),
         new Result(Party.UDMR, 250, 5),
      ];

      it("should combine the last contenders into others group", () => {
         const expectedResults = [
            new Result(Party.PNL, 2500, 50),
            new Result(Party.OTHERS, 500, 10),
            new Result(Party.PSD, 2000, 40)
         ];

         const election = new Election("council", results, 5000, 10000);
         const wrapper = shallow(<BarElectionResults election={election} showFirst={2}/> );

         expect(wrapper.find(HorizontalStackedBar).props().results).toEqual(expectedResults);
      });

      it("should not show others if showFirst number is too large", () =>{
         const expectedResults = [
            new Result(Party.PNL, 2500, 50),
            new Result(Party.PMP, 250, 5),
            new Result(Party.UDMR, 250, 5),
            new Result(Party.PSD, 2000, 40)
         ];

         const election = new Election("council", results, 5000, 10000);
         const wrapper = shallow(<BarElectionResults election={election} showFirst={3}/> );

         expect(wrapper.find(HorizontalStackedBar).props().results).toEqual(expectedResults);
      })
   });
});