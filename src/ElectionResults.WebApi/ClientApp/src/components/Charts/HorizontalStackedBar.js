import React, {useEffect} from "react"
import PropTypes from "prop-types"
import * as d3Select from "d3-selection";
import * as d3Scale from "d3-scale";
import {Result} from "../../domain/Result";
import  "./HorizontalStackedBar.css"


export const HorizontalStackedBar =  ({results}) => {

    useEffect(() => {
        drawBar();
    }, []);

    const marginBottom = 10;

    const chartWidth = 1000;
    const chartHeight = 80;
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
        const svg = d3Select.select(".horizontal-stacked-bar")
            .append("svg")
            .attr("viewBox", `0 0 ${chartWidth} ${chartHeight}`)
            .attr("preserveAspectRatio", "xMidYMid meet");

        const {stackedBarData, total} = stackResults(results);

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


    return <div className={"horizontal-stacked-bar"}/>
};

const stackResults = (results, shuffleFunc) => {
    return results
        .reduce(({stackedBarData, total}, result) => {
            stackedBarData.push({value: result.votes, cumulative: total, data: result});
            return {stackedBarData: stackedBarData, total: total + result.votes};
        }, {stackedBarData: [], total: 0});
};

HorizontalStackedBar.propTypes = {
    results: PropTypes.arrayOf(PropTypes.instanceOf(Result)).isRequired,
    height: PropTypes.number.isRequired,
    width: PropTypes.number.isRequired
};