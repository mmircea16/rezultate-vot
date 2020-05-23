import React, {useEffect} from "react"
import PropTypes from "prop-types"
import * as d3Select from "d3-selection";
import * as d3Scale from "d3-scale";
import {Result} from "../../domain/Result";
import  "./HorizontalStackedBar.css"


export const HorizontalStackedBar =  ({results, width, height, orderType}) => {

    useEffect(() => {
        drawBar();
    }, []);

    const marginBottom = 10;

    const chartWidth = width;
    const chartHeight = height;
    const barHeight = chartHeight - 2*marginBottom;
    const halfBarHeight = barHeight / 2;

    const putLabels = function (svg, leftLabel, rightLabel) {
        const textMargin = 5;
        const yBottomChart = chartHeight - marginBottom;

        svg.append('text')
            .attr('class', 'text-value')
            .attr('x', textMargin)
            .attr('y', yBottomChart - textMargin)
            .text(leftLabel);


        svg.append('text')
            .attr('class', 'text-value')
            .attr('text-anchor', 'end')
            .attr('x', chartWidth - textMargin)
            .attr('y', yBottomChart - textMargin)
            .text(rightLabel)
    };

    const drawLineInTheMiddle = function (svg, middle) {
        svg.append('line')
            .attr("x1", middle)
            .attr("x2", middle)
            .attr("y1", 0)
            .attr("y2", chartHeight)
            .style("stroke-width", 2)
            .style("stroke", "black");
    };

    const drawBar = () => {
        const svg = d3Select.select(".mychart")
            .append("svg")
            .attr("width", chartWidth)
            .attr("height", chartHeight)
            .style("margin-left", 100);

        const {stackedBarData, total} = stackResults(results, ORDER_TYPES[orderType]);

        const xScale = d3Scale.scaleLinear()
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
            .style('fill', d => d.data.entity.color);

        drawLineInTheMiddle(svg, xScale(total / 2));

        const len = stackedBarData.length;
        putLabels(svg, stackedBarData[0].value, stackedBarData[len-1].value);
    };


    return <div className={"mychart"}>Horizontal stacked bar</div>
};

const sortAsc = function (results) {
    return results.sort((a, b) => a.votes - b.votes);
};

const alternate = function(results) {
    let rearrangedResults = results
        .sort((a, b) => b.votes - a.votes)
        .map((result, index) => ({...result, votes: index % 2 ? result.votes : (-1)*result.votes}));

    console.log(rearrangedResults);
    return rearrangedResults
        .sort((a, b) => a.votes - b.votes)
        .map(result => ({...result, votes: Math.abs(result.votes)}));
};

const stackResults = (results, shuffleFunc) => {
    const sortedResults = shuffleFunc(results);

    return sortedResults
        .reduce(({stackedBarData, total}, result) => {
            stackedBarData.push({value: result.votes, cumulative: total, data: result});
            return {stackedBarData: stackedBarData, total: total + result.votes};
        }, {stackedBarData: [], total: 0});
};

const ORDER_TYPES = {
    "ASC": sortAsc,
    "ALTERNATE": alternate,
};

HorizontalStackedBar.propTypes = {
    results: PropTypes.arrayOf(PropTypes.instanceOf(Result)).isRequired,
    height: PropTypes.number.isRequired,
    width: PropTypes.number.isRequired,
    orderType: PropTypes.oneOf(ORDER_TYPES.keys)
};