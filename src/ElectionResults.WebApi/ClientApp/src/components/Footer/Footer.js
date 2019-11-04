import React from "react";
import { Container, Row, Col, Label, Input, Media } from "reactstrap";
import code4Ro from "../../images/code4RoGrey.svg";
import "./Footer.css";

export function Footer() {
  return (
    <footer>
      <Container className="text-white footer">
        <Row>
          <Col style={{ alignSelf: "flex-end" }}>
            <h4 className="link-title">Linkuri Utile</h4>
            <div>
              <a className="link-item" href="#">
                Termeni și condiții
              </a>
            </div>
            <div>
              <a className="link-item" href="#">
                Politica de confidențialitate
              </a>
            </div>
            <div>
              <a className="link-item" href="#">
                Codul de conduita
              </a>
            </div>
            <div>
              <a className="link-item" href="#">
                Code for România
              </a>
            </div>
            <div>
              <a className="link-item" href="#">
                Contact
              </a>
            </div>
          </Col>
          <Col>
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
            <div className="text-right mb-2">&copy; 2019 Code for Romania.</div>
            <div className="text-right">
              Organizație neguvernamentală independentă, neafiliată politic și
              apolitică.
            </div>
          </Col>
        </Row>
      </Container>
    </footer>
  );
}
