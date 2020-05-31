import {Election} from "../domain/Election";
import {Result} from "../domain/Result";
import {Party} from "../domain/Party";

export const HistoricResultsService = {
    getResults(type, id, administrativeUnit) {
        console.log("get results for", administrativeUnit);
        const results = [
            new Result(Party.PSD, 2000000, 36),
            new Result(Party.PNL, 1800000, 32.7),
            new Result(Party.USR, 700000, 12.7),
            new Result(Party.UDMR, 350000, 6.3),
            new Result(Party.PMP, 300000, 5.4),
            new Result(Party.ALDE, 200000, 3.6),
            new Result(Party.PRORO, 180000, 3.3)
        ];

        return new Election(type, results, 1000, 1500, id);
    }
};