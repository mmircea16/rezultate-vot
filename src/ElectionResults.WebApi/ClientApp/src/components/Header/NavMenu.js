import React, { Component } from 'react';
import { Collapse, Nav, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink, Container } from 'reactstrap';
import { Link } from 'react-router-dom';
import votLogo from '../../images/rezultateVot.png';
import './NavMenu.css';
import ElectionPicker from '../../services/electionPicker';

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

  loadElectionRound(event) {
      ElectionPicker.changeSelection(event.target.value);
  }

  render() {
    return (
      <header>
        <Navbar light expand="md">
          <Container>
            <NavbarBrand tag={Link} to="/web">
              <img src={votLogo} width={90} height={80} alt="Rezultate Vot" />
            </NavbarBrand>
            <NavbarToggler className="mr-2 menu-toggle" onClick={this.toggleNavbar} />
            <Collapse isOpen={!this.state.collapsed} navbar>
                <Nav className="ml-auto" navbar>
                    <NavItem>
                        <select className="header-select" onChange={this.loadElectionRound}>
                            <option value="prezidentiale24112019">TURUL 2</option>
                            <option value="prezidentiale10112019">TURUL 1</option>
                        </select>
                    </NavItem>
                    <NavItem>
                      <NavLink tag={Link} className="text-dark" to="/web/despre-proiect">DESPRE PROIECT</NavLink>
                    </NavItem>
                    <NavItem>
                      <NavLink tag={Link} className="text-dark" to="/web/despre-noi">DESPRE NOI</NavLink>
                    </NavItem>
              </Nav>
            </Collapse>
          </Container>
        </Navbar>
      </header>
    );
  }
}
