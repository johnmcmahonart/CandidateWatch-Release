
import React, { useCallback, useState } from "react";
import { PieChart, Pie, Cell, ResponsiveContainer } from "recharts";
import { IPieChartProps } from "../Interfaces/Components";

export default function DefaultPieChart(props: IPieChartProps) {
    return (
        <ResponsiveContainer width="100%" height={200}>
        <PieChart>
            <Pie
                data={props.chartData}
                
                labelLine={false}
                outerRadius={80}
                fill="#8884d8"
                dataKey="dataKey"
            >
                
            </Pie>
        </PieChart>
</ResponsiveContainer >
    );
}


