import React from 'react';

import { Item } from './Item'
import Grid2 from '@mui/material/Unstable_Grid2';

import useTop5PACDataBuilder from '../Hooks/useTop5PACDataBuilder';
import { IChartData } from '../Interfaces/Components';
import { isChartData } from '../Utilities';
import useTop5CandidateDonationDataBuilder from '../Hooks/useTop5CandidateDonationDataBuilder';
import DefaultBarChart from './DefaultBarChart';
import useIndividualVsPACContributionsDataBuilder from '../Hooks/useIndividualVsPACContributionsDataBuilder';
import DefaultPieChart from './DefaultPieChart';
export default function CandidateDetailCharts() {
    const top5ChartData = useTop5PACDataBuilder();
    const top5CandidateDonationData = useTop5CandidateDonationDataBuilder();
    const individualVsPACData = useIndividualVsPACContributionsDataBuilder();

    
    if (isChartData(top5ChartData) && isChartData(top5CandidateDonationData) && isChartData(individualVsPACData)) {
        
        
        return (
            <>
                <Grid2 xs={8} sm={6}>
                    <Item>
                        <DefaultBarChart chartData={top5ChartData} isVertical={true} margin={{
                            top: 10,
                            right: 30,
                            left: 50,
                            bottom: 0
                        }} />
                    </Item>

                </Grid2>
                <Grid2 xs={8} sm={4}>
                    <Item>
                        <DefaultPieChart chartData={individualVsPACData} margin={{
                            top: 10,
                            right: 30,
                            left: 50,
                            bottom: 0
                        }} />
                    </Item>

                </Grid2>
                <Grid2 xs={8} sm={6}>
                    <Item>
                        <DefaultBarChart chartData={top5CandidateDonationData} isVertical={false} margin={{
                            top: 10,
                            right: 30,
                            left: 50,
                            bottom: 0
                        }} />
                    </Item>

                </Grid2>
                
            </>

        )
    }
    else {
        return (<div></div>)
    }   
}