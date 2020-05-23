export class Result {
    entity;
    votes;
    percentage;

    constructor(entity, votes, percentage) {
        this.entity = entity;
        this.votes = votes;
        this.percentage = percentage;
    }
}