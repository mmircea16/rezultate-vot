class PoliticalParty {
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
    PSD: new PoliticalParty("PSD", "red", ""),
    PNL: new PoliticalParty("PNL", "blue", ""),
    USR: new PoliticalParty("USR", "light blue", ""),
    UDMR: new PoliticalParty("UDMR", "green", ""),
    PMP: new PoliticalParty("PMP", "red", ""),
    PRORO: new PoliticalParty("PRORO", "light green", ""),
    ALDE: new PoliticalParty("ALDE", "yellow", ""),
};
