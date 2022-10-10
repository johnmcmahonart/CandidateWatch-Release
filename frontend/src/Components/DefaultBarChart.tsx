import React from 'react';
import { Bar, BarChart, CartesianGrid, Tooltip, XAxis, YAxis, ResponsiveContainer } from "recharts";
import { IBarChartProps, IChartData, IChartMargin } from "../Interfaces/Components";
import ChartToolTip from "./ChartToolTip";

export default function DefaultBarChart(props: IBarChartProps) {
    return (

        <ResponsiveContainer width="100%" height={200}>

            
            {props.isVertical ? (
                <BarChart data={props.chartData} layout="vertical" margin={props.margin}>
                   
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis type="number" />
                    <YAxis type="category" dataKey="labelShort" />

                    <Tooltip content={<ChartToolTip payload={props.chartData} />} />
                    <Bar dataKey="dataKey" fill="#444444" />

                </BarChart>) : (
                <BarChart data={props.chartData} margin={props.margin}>

                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis type="category" dataKey="labelShort" />
                    <YAxis type="number" />

                    <Tooltip content={<ChartToolTip payload={props.chartData} />} />
                    <Bar dataKey="dataKey" fill="#444444" />

                </BarChart>)
            }

        </ResponsiveContainer>

    )
}