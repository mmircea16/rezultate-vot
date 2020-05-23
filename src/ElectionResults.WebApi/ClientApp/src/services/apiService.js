let base_URL = "https://rezultatevot.ro"

export const getVoterTurnoutUrl = (electionId) => `${base_URL}/api/results/voter-turnout?electionId=${electionId}`

export const getElectionConfigUrl = () => `${base_URL}/api/settings/election-config`

export const getElectionResultsUrl = (electionId, source, county) => (!source) ? `${base_URL}/api/results?electionId=${electionId}` : `${base_URL}/api/results?electionId=${electionId}&source=${source}&county=${county || ''}`

export const getVoteMonitoringUrl = (electionId) => `${base_URL}/api/results/monitoring?electionId=${electionId}`
