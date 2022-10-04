import React from 'react';
import { useSelector } from 'react-redux';
import { useGetApiScheduleBDetailByKeyYearsQuery, ScheduleBDetailDto } from '../APIClient/MDWatch';
//import { RootState } from '../Redux/store';
import { selectData } from '../Redux/UISelection';
import CircularProgress from '@mui/material/CircularProgress';
import { IChartToolTip, IChartData, ILineChart } from '../Interfaces/Components';
import { nanoid } from '@reduxjs/toolkit';
import { from, IEnumerable, initializeLinq } from 'linq-to-typescript';
import { Bar, BarChart, CartesianGrid, XAxis, YAxis, Text, LabelList, ResponsiveContainer, Tooltip } from 'recharts';
import { StringNotEmpty } from '../Utilities';
import PrepareLabelsforDisplay from '../PrepareLabels';
import Grid2 from '@mui/material/Unstable_Grid2';
import ChartToolTip from './ChartToolTip';
export default function CandidateDetailCharts() {
    const uiSelectionData = useSelector(selectData);
    const scheduleBData = useGetApiScheduleBDetailByKeyYearsQuery({ key: uiSelectionData.candidateId, years: [uiSelectionData.electionYear] });
    const buildTop5ChartData = (source: any): IChartData[]  => {
        const mappedData: Array<IChartData> = [];

        source.data.map((element: ScheduleBDetailDto) => (
            mappedData.push({
                label: StringNotEmpty(element.committeeName),
                dataKey: element.total,
                labelShort: StringNotEmpty(element.committeeName),
                xAxisLabel: "Dollars",
                yAxisLabel:"PAC Name"
            })
        ))

        const chartData: Array<IChartData> = PrepareLabelsforDisplay(mappedData.orderBy((x => x.dataKey)).reverse().take(5).toArray(), true,10);
        
        return chartData;
    }
    if (scheduleBData.isSuccess) {
        const chartData = buildTop5ChartData(scheduleBData);

        return (

            <Grid2 container>
                <div style={{ width: "100%", height: 200 }}>
                    <ResponsiveContainer>
                        <BarChart data={chartData} layout="vertical" margin={{
                            top: 10,
                            right: 30,
                            left: 100,
                            bottom: 0
                        }}>

                            <CartesianGrid strokeDasharray="3 3" />
                            <XAxis type="number" />
                            <YAxis type="category" dataKey="labelShort"/>


                            <Tooltip content={<ChartToolTip payload={chartData}/>}/>
                            <Bar dataKey="dataKey" fill="#444444" />

                        </BarChart>
                    </ResponsiveContainer>

                </div>
            </Grid2>




        )
    }

    else if (scheduleBData.isLoading || scheduleBData.isFetching) {
        return (

            <CircularProgress />
        )
    }
    return (
        <CircularProgress />
    )
}