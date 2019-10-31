import React, { Component } from 'react';
import { Collapse, Nav, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink, Media } from 'reactstrap';
import { Link } from 'react-router-dom';
import votLogo from '../../images/rezultateVot.png';
import './NavMenu.css';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor(props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true
    };
  }

  toggleNavbar() {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render() {
    return (
      <header>
        <Navbar color="light" light expand="md">
          <NavbarBrand tag={Link} to="/">
            <img src={votLogo} width={80} height={80} alt="Rezultate Vot" />
          </NavbarBrand>
          <NavbarToggler className="mr-2 menu-toggle" onClick={this.toggleNavbar} />
          <Collapse isOpen={!this.state.collapsed} navbar>
            <Nav className="ml-auto" navbar>
              <NavItem>
                <NavLink tag={Link} className="text-white" to="/">ISTORIC VOT</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-white" to="/">PREZENȚA LA VOT</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-white" to="/">DESPRE NOI</NavLink>
              </NavItem>
            </Nav>
          </Collapse>
        </Navbar>
      </header>
    );
  }
}
