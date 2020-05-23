export class Election {
    total;
    type;
    results;
    turnout;

    constructor(type, results, turnout, total) {
        this.type = type;
        this.results = results;
        this.turnout = turnout;
        this.total = total;
    }
}