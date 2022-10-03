import React from 'react';
import { useSelector } from 'react-redux';
import { useGetApiScheduleBDetailByKeyYearsQuery, ScheduleBDetailDto } from '../APIClient/MDWatch';
//import { RootState } from '../Redux/store';
import { selectData } from '../Redux/UISelection';
import CircularProgress from '@mui/material/CircularProgress';
import { ILineChartData } from '../Interfaces/Components';
import { nanoid } from '@reduxjs/toolkit';
import { from, IEnumerable, initializeLinq } from 'linq-to-typescript';
import { Bar, BarChart, CartesianGrid, XAxis, YAxis, Text, LabelList, ResponsiveContainer } from 'recharts';
import { StringNotEmpty } from '../Utilities';
import PrepareLabelsforDisplay from '../PrepareLabels';
import Grid2 from '@mui/material/Unstable_Grid2';
export default function CandidateDetailCharts() {
    const uiSelectionData = useSelector(selectData);
    const scheduleBData = useGetApiScheduleBDetailByKeyYearsQuery({ key: uiSelectionData.candidateId, years: [uiSelectionData.electionYear] });
    const buildTop5ChartData = (source: any): ILineChartData[] => {
        const mappedData: Array<ILineChartData> = [];

        source.data.map((element: ScheduleBDetailDto) => (
            mappedData.push({ label:StringNotEmpty(element.committeeName), dataKey: element.total })
        ))

        const result: Array<ILineChartData> = PrepareLabelsforDisplay(mappedData.orderBy((x => x.dataKey)).reverse().take(5).toArray(), true);
        return result;
    }
    if (scheduleBData.isSuccess) {
        const data = buildTop5ChartData(scheduleBData);
        
        return (
            
            <Grid2 container>
                <div style={{ width: "100%", height: 200 }}>
                    <ResponsiveContainer>
                        <BarChart data={data} layout="vertical" margin={{
                            top: 10,
                            right: 30,
                            left: 50,
                            bottom: 0
                        }}>

                            <CartesianGrid strokeDasharray="3 3" />
                            <XAxis type="number" />
                            <YAxis type="category" dataKey="label" label={{position:'inside'} }/>

                            
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