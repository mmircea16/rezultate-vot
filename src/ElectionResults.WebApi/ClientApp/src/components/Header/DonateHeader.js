import React from 'react';
import { Button, Media, Label, Container } from 'reactstrap';
import code4Ro from '../../images/code4Romania.svg';

export function DonateHeader() {
  return (
    <div>
      <Container style={{ display: 'flex', alignItems: 'center', justifyContent: 'flex-end' }}>
        <Label className="info-label">an app developed by</Label>
        <Media src={code4Ro} />
        <a href="https://code4.ro/ro/doneaza/">
          <Button color="success">Donează</Button>
        </a>
      </Container>
    </div>
  )
}
