import React, { Component } from "react";
import "./scss/reset.css";
import { Route } from "react-router";
import { Layout } from "./components/Layout";
import { FetchData } from "./components/FetchData";
import { Counter } from "./components/Counter";
import { HomePage } from "./components/HomePage";

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <Layout>
        <Route exact path="/" component={HomePage} />
        <Route path="/counter" component={Counter} />
        <Route path="/fetch-data" component={FetchData} />
      </Layout>
    );
  }
}
