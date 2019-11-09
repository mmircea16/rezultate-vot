import React from 'react';
import styled from 'styled-components';
import { Button, Media, Label } from 'reactstrap';
import code4Ro from '../../images/code4Romania.svg';

const Container = styled.div`
  width: 100%;
  padding-right: 15px;
  padding-left: 15px;
  margin-right: auto;
  margin-left: auto;
  display: flex;
  align-items: center;
  justify-content: flex-end;
`;

export function DonateHeader() {
  return (
    <div>
      <Container>
        <Label className="info-label">an app developed by</Label>
        <Media src={code4Ro} />
        <a href="https://code4.ro/ro/doneaza/">
          <Button color="success">Donează</Button>
        </a>
      </Container>
    </div>
  )
}
