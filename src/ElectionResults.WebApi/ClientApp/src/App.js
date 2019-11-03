import React, { Component } from "react";
import { Route } from "react-router";
import { Layout } from "./components/Layout";
import { ChartContainer } from "./components/CandidatesChart";
import { AdminPanel } from "./components/AdminPanel/AdminPanel";

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <Layout>
        <Route exact path="/" component={ChartContainer} />
        <Route path="/admin" component={AdminPanel} />
      </Layout>
    );
  }
}
