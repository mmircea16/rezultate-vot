import React from 'react';
import styled from 'styled-components';
import { Button, Media } from 'reactstrap';
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
        <Media src={code4Ro} />
        <Button color="success">Donează</Button>
      </Container>
    </div>
  )
}
