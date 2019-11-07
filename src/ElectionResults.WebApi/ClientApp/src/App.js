import React, { Component } from "react";
import { Route } from "react-router";
import { Layout } from "./components/Layout";
import { AdminPanel } from "./components/AdminPanel/AdminPanel";
import { ConfidentialityPolicyPage } from "./components/ConfidentialityPolicyPage/ConfidentialityPolicyPage";
import { TermsAndConditionsPage } from "./components/TermsAndConditionsPage/TermsAndConditionsPage";
import { HomePage } from "./components/HomePage";

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <Layout>
        <Route exact path="/" component={HomePage} />
        <Route path="/admin" component={AdminPanel} />
        <Route path="/politica-de-confidentialitate" component={ConfidentialityPolicyPage} />
        <Route path="/termeni-si-conditii" component={TermsAndConditionsPage} />
      </Layout>
    );
  }
}
