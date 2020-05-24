import React from 'react';
import { shallow } from 'enzyme';
import {BarElectionResults} from "./BarElectionResults";
import {Result} from "../../../domain/Result";
import {Party} from "../../../domain/Party";
import {Election} from "../../../domain/Election";
import {HorizontalStackedBar} from "../../Charts/HorizontalStackedBar";

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
      const wrapper = shallow(<BarElectionResults electionResults={election}/> );

      expect(wrapper.find(HorizontalStackedBar).props().results).toEqual(expectedOrderResults);
   })
});