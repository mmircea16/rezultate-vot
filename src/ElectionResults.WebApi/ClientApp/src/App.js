import React, {Component} from "react";
import {Route, Redirect} from "react-router";
import {Layout} from "./components/Layout";
import {AboutProjectPage} from "./components/AboutProjectPage/AboutProjectPage";
import {AboutUsPage} from "./components/AboutUsPage/AboutUsPage";
import {ConfidentialityPolicyPage} from "./components/ConfidentialityPolicyPage/ConfidentialityPolicyPage";
import {TermsAndConditionsPage} from "./components/TermsAndConditionsPage/TermsAndConditionsPage";
import {HomePage} from "./components/HomePage";
import {VoteMonitoring} from "./components/VoteMonitoring";
import {ChartContainer} from "./components/CandidatesChart";
import {ElectionChart} from "./components/Chart";

export default class App extends Component {
    static displayName = App.name;

    render() {
        const baseNonWidgetPath = '/web';
        const basWidgetPath = '/widgets';
        return (
            <div>
                <Route exact path="/" render={() => (
                    <Redirect to={baseNonWidgetPath}/>
                )} />
                <Route path={baseNonWidgetPath} component={HomePage}>
                    <Layout>
                        <Route exact path={`${baseNonWidgetPath}/`} component={HomePage}/>
                        <Route path={`${baseNonWidgetPath}/despre-proiect`} component={AboutProjectPage}/>
                        <Route path={`${baseNonWidgetPath}/despre-noi`} component={AboutUsPage}/>
                        <Route path={`${baseNonWidgetPath}/politica-de-confidentialitate`} component={ConfidentialityPolicyPage}/>
                        <Route path={`${baseNonWidgetPath}/termeni-si-conditii`} component={TermsAndConditionsPage}/>
                        <Route path={`${baseNonWidgetPath}/monitorizare-vot`} component={VoteMonitoring}/>
                        <Route path={`${baseNonWidgetPath}/grafice-alegeri`} component={ChartContainer}/>
                        <Route path={`${baseNonWidgetPath}/rezultate-alegeri`} component={ElectionChart}/>
                    </Layout>
                </Route>
                <Route path={basWidgetPath}>
                    <div class="widget-container">
                        <Route path={`${basWidgetPath}/rezultate-alegeri`} component={ChartContainer}/>
                        <Route path={`${basWidgetPath}/monitorizare-vot`} component={VoteMonitoring}/>
                        <Route path={`${basWidgetPath}/prezenta-alegeri`} component={ElectionChart}/>
                    </div>
                </Route>
            </div>
        );
    }
}
