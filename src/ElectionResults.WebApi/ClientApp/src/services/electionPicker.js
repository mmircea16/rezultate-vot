"use strict";
let currentElectionId = 'prezidentiale24112019';
let isInWidget = true;

function getQueryId() {
    let search = window.location.search;
    let params = new URLSearchParams(search);
    return params.get('electionId');
}

export class ElectionPicker {

    static changeSelection(id) {
        currentElectionId = id;
        var queryElectionId = getQueryId();
        if (queryElectionId)
            currentElectionId = queryElectionId;
        window.dispatchEvent(new Event("selectedElectionsChanged", { bubbles: true, cancelable: false }));
    };

    static setDefaultElectionId(id) {
        isInWidget = false;
        currentElectionId = id;
    }

    static getSelection() {

        if (isInWidget) {
            var queryId = getQueryId();
            if (!queryId)
                currentElectionId = 'prezidentiale10112019';
            else currentElectionId = queryId;
        }
        return currentElectionId;
    }

}

export default ElectionPicker;