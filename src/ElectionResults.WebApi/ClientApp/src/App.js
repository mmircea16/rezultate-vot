import React, { Component } from "react";
import { Route } from "react-router";
import { Layout } from "./components/Layout";
import { AboutProjectPage } from "./components/AboutProjectPage/AboutProjectPage";
import { AboutUsPage } from "./components/AboutUsPage/AboutUsPage";
import { ConfidentialityPolicyPage } from "./components/ConfidentialityPolicyPage/ConfidentialityPolicyPage";
import { TermsAndConditionsPage } from "./components/TermsAndConditionsPage/TermsAndConditionsPage";
import { HomePage } from "./components/HomePage";
import { VoteMonitoring } from "./components/VoteMonitoring";
import { ChartContainer } from "./components/CandidatesChart";
import { ElectionChart } from "./components/Chart";

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <Layout>
        <Route exact path="/" component={HomePage} />
        <Route path="/despre-proiect" component={AboutProjectPage} />
        <Route path="/despre-noi" component={AboutUsPage} />
        <Route path="/politica-de-confidentialitate" component={ConfidentialityPolicyPage} />
        <Route path="/termeni-si-conditii" component={TermsAndConditionsPage} />
        <Route path="/monitorizare-vot" component={VoteMonitoring} />
        <Route path="/grafice-alegeri" component={ChartContainer} />
        <Route path="/rezultate-alegeri" component={ElectionChart} />
      </Layout>
    );
  }
}
