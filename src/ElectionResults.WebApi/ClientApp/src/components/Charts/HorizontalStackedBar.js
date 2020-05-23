import React, {useEffect} from "react"
import PropTypes from "prop-types"
import * as d3 from "d3";
import {Result} from "../../domain/Result";


export const HorizontalStackedBar =  ({results}) => {

    useEffect(() => {
        drawBar();
    }, []);


    const chartWidth = 300;
    const chartHeight = 100;
    const barHeight = 80;
    const halfBarHeight = barHeight / 2;
    const colors = ['#e41a1c', '#377eb8', '#4daf4a', '#984ea3', '#ff7f00', '#ffff33'];

    const drawBar = () => {
        const svg = d3.select(".mychart")
            .append("svg")
            .attr("width", chartWidth)
            .attr("height", chartHeight)
            .style("margin-left", 100);

        const {stackedBarData, total} = stackResults(results);

        const xScale = d3.scaleLinear()
            .domain([0, total])
            .range([0, chartWidth]);

        svg.selectAll('rect')
            .data(stackedBarData)
            .enter().append('rect')
            .attr('class', 'rect-stacked')
            .attr('x', d => xScale(d.cumulative))
            .attr('y', chartHeight / 2 - halfBarHeight)
            .attr('height', barHeight)
            .attr('width', d => xScale(d.value))
            .style('fill', d => d.data.entity.color)
    };


    return <div className={"mychart"}>Horizontal stacked bar</div>
};

const stackResults = (results) => {
    const sortedResults = results
        .sort((a, b) => a.votes - b.votes);

    return sortedResults
        .reduce(({stackedBarData, total}, result) => {
            stackedBarData.push({value: result.votes, cumulative: total, data: result});
            return {stackedBarData: stackedBarData, total: total + result.votes};
        }, {stackedBarData: [], total: 0});
};

HorizontalStackedBar.propTypes = {
    results: PropTypes.arrayOf(PropTypes.instanceOf(Result))
};