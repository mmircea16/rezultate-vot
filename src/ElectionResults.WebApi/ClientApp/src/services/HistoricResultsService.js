import {Election} from "../domain/Election";

export const HistoricResultsService = {
    getResults(type, id, administrativeUnit) {
        return new Election(type, [], 1000, 1500);
    }
}