import React, { Component } from "react";
import { Route } from "react-router";
import { Layout } from "./components/Layout";
import { ChartContainer } from "./components/CandidatesChart";
import { FetchData } from "./components/FetchData";
import { Counter } from "./components/Counter";
import { AdminPanel } from "./components/AdminPanel/AdminPanel";

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <Layout>
        <Route exact path="/" component={ChartContainer} />
        <Route path="/counter" component={Counter} />
        <Route path="/fetch-data" component={FetchData} />
        <Route path="/admin" component={AdminPanel} />
      </Layout>
    );
  }
}
