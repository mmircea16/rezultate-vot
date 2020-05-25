export class Election {
    total;
    type;
    results;
    turnout;
    name;

    constructor(type, results, turnout, total, name) {
        this.type = type;
        this.results = results;
        this.turnout = turnout;
        this.total = total;
        this.name = name;
    }
}