import React from 'react';
import { useSelector } from 'react-redux';
import { useGetApiScheduleBDetailByKeyYearsQuery, ScheduleBDetailDto } from '../APIClient/MDWatch';
//import { RootState } from '../Redux/store';
import { selectData } from '../Redux/UISelection';
import CircularProgress from '@mui/material/CircularProgress';
import { ILineChartData } from '../Interfaces/Components';
import { nanoid } from '@reduxjs/toolkit';
import { from, IEnumerable, initializeLinq } from 'linq-to-typescript';
import { Bar, BarChart, CartesianGrid, XAxis, YAxis } from 'recharts';
export default function CandidateDetailCharts() {
    
    const uiSelectionData = useSelector(selectData);
    const scheduleBData = useGetApiScheduleBDetailByKeyYearsQuery({ key: uiSelectionData.candidateId, years: [uiSelectionData.electionYear] });
    const buildTop5ChartData = (source: any): ILineChartData[] => {
        const mappedData: Array<ILineChartData> = [];

        source.data.map((element: ScheduleBDetailDto) => (
            mappedData.push({ xAxisLabel: element.committeeName, dataKey: element.total })
        ))

        const result: Array<ILineChartData> = mappedData.orderBy((x => x.dataKey)).reverse().take(5).toArray();
        return result;
    }
    if (scheduleBData.isSuccess) {
        const data = buildTop5ChartData(scheduleBData);
        console.log(data);
        return (
            <BarChart width={1000} height={300} data={data} layout="horizontal" >
                
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="xAxisLabel" />
                <Bar dataKey="dataKey" fill="#444444">
                <Text scaleToFit=true/>
                </Bar
            </BarChart>

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