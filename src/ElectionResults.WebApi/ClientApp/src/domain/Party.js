export class PoliticalParty {
    name;
    color;
    emblem;

    constructor(name, color, emblem) {
        this.name = name;
        this.color = color;
        this.emblem = emblem;
    }
}

export const Party = {
    PSD: new PoliticalParty("PSD", "#ff0000", ""),
    PNL: new PoliticalParty("PNL", "#0000ff", ""),
    USR: new PoliticalParty("USR", "#00f0ff", ""),
    UDMR: new PoliticalParty("UDMR", "#00ff00", ""),
    PMP: new PoliticalParty("PMP", "#0060aa", ""),
    PRORO: new PoliticalParty("PRORO", "#00a000", ""),
    ALDE: new PoliticalParty("ALDE", "#ffff00", ""),
};
