import React from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './Header/NavMenu';
import { DonateHeader } from './Header/DonateHeader';
import { Footer } from './Footer/Footer';

export function Layout(props) {
  return (
    <div>
      <NavMenu />
      <DonateHeader />
      <Container>
        {props.children}
      </Container>
      <Footer />
    </div>
  )
}
