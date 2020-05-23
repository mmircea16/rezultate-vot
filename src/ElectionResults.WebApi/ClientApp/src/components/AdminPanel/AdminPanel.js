import React, { useState, useEffect } from 'react';
import { Label, Input } from 'reactstrap';
import { getElectionConfigUrl } from '../services/apiService';


const AdminPanel = () => {
  // const API_URL = '/api/settings/election-config';
  const [config, setConfig] = useState({ Files: [], Candidates: [] });

  useEffect(() => {
    fetch(getElectionConfigUrl())
      .then(data => data.json())
      .then(data => setConfig(data));
  }, []);

  const save = () => {
    fetch(getElectionConfigUrl(), {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(config),
    });
  }

  const addFile = () => {
    const emptyFile = { URL: '', Name: '', Active: false, ResultsLocation: 0, ResultsType: 0 };
    setConfig({ ...config, Files: config.Files.concat(emptyFile) });
  }

  const removeFile = (index) => {
    const filesCopy = Array.from(config.Files);
    filesCopy.splice(index, 1);
    setConfig({ ...config, Files: filesCopy });
  }

  const handleFileChange = (event, index) => {
    const numberFields = ['ResultsType', 'ResultsLocation'];
    const filesCopy = Array.from(config.Files);
    filesCopy[index][event.target.name] = numberFields.includes(event.target.name)
      ? parseInt(event.target.value, 10)
      : event.target.value;
    setConfig({ ...config, Files: filesCopy });
  }

  const addCandidate = () => {
    const emptyCandidate = { Name: '', ImageUrl: '', CsvId: '' };
    setConfig({ ...config, Candidates: config.Candidates.concat(emptyCandidate) });
  }

  const removeCandidate = (index) => {
    const candidatesCopy = Array.from(config.Candidates);
    candidatesCopy.splice(index, 1);
    setConfig({ ...config, Candidates: candidatesCopy });
  }

  const handleCandidateChange = (event, index) => {
    const candidatesCopy = Array.from(config.Candidates);
    candidatesCopy[index][event.target.name] = event.target.value;
    setConfig({ ...config, Candidates: candidatesCopy });
  }

  return (
    <div>
      <form>
        {config.Files.map((file, index) => {
          return (
            <div key={index} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end' }}>
              <div className="form-group">
                <Label for="URL">BEC Url</Label>
                <Input type="text" name="URL" placeholder="BEC Url" bsSize="sm"
                  value={file.URL} onChange={(event) => handleFileChange(event, index)} />
              </div>
              <div className="form-group">
                <Label for="ResultsType">Result type</Label>
                <Input type="select" name="ResultsType" bsSize="sm"
                  value={file.ResultsType} onChange={(event) => handleFileChange(event, index)}>
                  <option value={0}>Provisional</option>
                  <option value={1}>Partial</option>
                  <option value={2}>Final</option>
                  <option value={3}>Turnout</option>
                  <option value={4}>VoteMonitoring</option>
                </Input>
              </div>
              <div className="form-group">
                <Label for="ResultsLocation">Location</Label>
                <Input type="select" name="ResultsLocation" bsSize="sm"
                  value={file.ResultsLocation} onChange={(event) => handleFileChange(event, index)}>
                  <option value={0}>Romania</option>
                  <option value={1}>Diaspora</option>
                  <option value={2}>All</option>
                </Input>
              </div>
              <div className="form-group">
                <Label for="Active" check>
                  <Input className="mt-0" type="checkbox" name="Active" value={file.Active}
                    style={{ width: '1em', height: '1em' }} />
                  Is active
                </Label>
              </div>
              <div className="form-group">
                <button type="button" className="btn btn-sm btn-secondary" onClick={() => removeFile(index)}>Remove</button>
              </div>
            </div>
          )
        })}
        <button type="button" className="btn btn-sm btn-secondary mb-3" onClick={addFile}>Add new URL</button>
        {config.Candidates.map((candidate, index) => {
          return (
            <div key={index} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end' }}>
              <div className="form-group">
                <Label for="Name">Name</Label>
                <Input type="text" name="Name" placeholder="Name" bsSize="sm"
                  value={candidate.Name} onChange={(event) => handleCandidateChange(event, index)} />
              </div>
              <div className="form-group">
                <Label for="ImageUrl">Image URL</Label>
                <Input type="text" name="ImageUrl" placeholder="Image URL" bsSize="sm"
                  value={candidate.ImageUrl} onChange={(event) => handleCandidateChange(event, index)} />
              </div>
              <div className="form-group">
                <Label for="CsvId">CSV Id</Label>
                <Input type="text" name="CsvId" placeholder="CSV Id" bsSize="sm"
                  value={candidate.CsvId} onChange={(event) => handleCandidateChange(event, index)} />
              </div>
              <div className="form-group">
                <button type="button" className="btn btn-sm btn-secondary" onClick={() => removeCandidate(index)}>Remove</button>
              </div>
            </div>
          )
        })}
        <button type="button" className="btn btn-sm btn-secondary mb-3" onClick={addCandidate}>Add new candidate</button>
      </form>
      <button type="button" className="btn btn-sm btn-success mb-3" onClick={save}>Save</button>
    </div>
  )
}

export { AdminPanel };
