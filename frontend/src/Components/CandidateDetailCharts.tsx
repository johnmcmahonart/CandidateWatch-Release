import React from 'react';
import Top5PACChart from './Top5PACChart';
import { Item } from './Item'
import Grid2 from '@mui/material/Unstable_Grid2';
import { Box } from '@mui/material';
import Top5CandidateDonationChart from './Top5CandidateDonation';
export default function CandidateDetailCharts() {
    return (
        <>
            <Grid2 xs={8} sm={6}>
                <Item>
                    <Top5PACChart />
                </Item>

            </Grid2>
            <Grid2 xs={8} sm={6}>
                <Item>
                    <Top5CandidateDonationChart />
                </Item>

                </Grid2>
        </>

    )
}