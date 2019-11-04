import React, { Component } from "react";
import { Route } from "react-router";
import { Layout } from "./components/Layout";
import { ChartContainer } from "./components/CandidatesChart";
import { AdminPanel } from "./components/AdminPanel/AdminPanel";
import { HomePage } from "./components/HomePage";

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <Layout>
        <Route exact path="/" component={HomePage} />
        <Route path="/admin" component={AdminPanel} />
      </Layout>
    );
  }
}
