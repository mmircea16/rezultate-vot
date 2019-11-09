import React from "react";
import { Container, Row, Col, Label, Input, Media } from "reactstrap";
import { Link } from 'react-router-dom';
import code4Ro from "../../images/code4RoGrey.svg";
import facebookLogo from "../../images/facebook_grey.png";
import instagramLogo from "../../images/instagram_grey.png";
import linkedinLogo from "../../images/linkedin_grey.png";
import twitterLogo from "../../images/twitter_grey.png";
import "./Footer.css";

export function Footer() {

  return (
      <footer>
      <Row>
        <Col
          className="social-media-container"
        >
          <a className="social-link-item" href="https://www.facebook.com/code4romania" target="_blank">
            <img src={facebookLogo} width={48} height={48} alt="Facebook" />
          </a>
          <a className="social-link-item" href="https://www.instagram.com/code4romania" target="_blank">
            <img src={instagramLogo} width={48} height={48} alt="Instagram" />
          </a>
          <a className="social-link-item" href="https://www.linkedin.com/company/code4romania" target="_blank">
            <img src={linkedinLogo} width={48} height={48} alt="LinkedIn" />
          </a>
          <a className="social-link-item" href="https://twitter.com/Code4Romania" target="_blank">
            <img src={twitterLogo} width={48} height={48} alt="Twitter" />
          </a>
          <a id="donate-link-button" href="https://code4.ro/ro/doneaza/" target="_blank">DONEAZĂ</a>
        </Col>
      </Row>
      <Container className="text-white footer">
        <Row>
          <Col
            xs="12"
            sm="6"
            className="usefull-links"
            style={{ alignSelf: "flex-end" }}
          >
            <h4 className="link-title">Linkuri Utile</h4>
            <div>
              <Link className="link-item" to="/termeni-si-conditii">
                Termeni și condiții
              </Link>
            </div>
            <div>
              <Link className="link-item" to="/politica-de-confidentialitate">
                Politica de confidențialitate
              </Link>
            </div>
            <div>
              <a className="link-item" target="_blank" href="https://code4.ro/ro/codul-de-conduita/">
                Codul de conduita
              </a>
            </div>
            <div>
              <a className="link-item" target="_blank" href="https://code4.ro/ro/">
                Code for Romania
              </a>
            </div>
            <div>
              <a className="link-item" href="mailto: contact@code4.ro">
                Contact
              </a>
            </div>
          </Col>
          <Col
            xs="12"
            sm="6"
          >
            <div className="form-group">
              <Label className="link-title" for="email">
                Abonează-te la newsletter
              </Label>
              <Input
                className="input-large"
                type="text"
                name="email"
                placeholder="Introdu adresa de e-mail si apasă ENTER"
              />
            </div>
            <div className="logo-container">
              <Media src={code4Ro} />
            </div>
            <div className="text-sm-right mb-2">&copy; 2019 Code for Romania.</div>
            <div className="text-sm-right">
              Organizație neguvernamentală independentă, neafiliată politic și
              apolitică.
            </div>
          </Col>
        </Row>
      </Container>
    </footer>
  );
}
