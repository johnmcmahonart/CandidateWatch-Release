import React from 'react';
import { useSelector } from 'react-redux';
import logo from './logo.svg';
import '../App.css';
import Grid2 from '@mui/material/Unstable_Grid2';
import { selectData, selectCaller } from '../Redux/UISelection';
import { CallerTypes } from '../Enums';
import CandidateDetailCharts from './CandidateDetailCharts';
import type { RootState, AppDispatch } from '../Redux/store'
import CandidateCardLoader from './CandidateCardLoader';
import CandidateCardLoaderParent from './CandidateCardLoaderParent';
import CandidateCardLoaderChild from './CandidateCardLoaderChild';
import Box from '@mui/material/Box';
//wrapper for MainContent area, so the children can be swapped out when the user selects a navigation item
export default function MainContentContainer() {
    const [open, setOpen] = React.useState(false);

    const uiSelectionCaller = useSelector(selectCaller);

    return (
        <Box sx={{backgroundColor:'#eeeeee'} }>
            <Grid2 container spacing={1} columns={8}>
                <Grid2 xs={8} key="gridSwitch" padding={2}>

                    {(function () {
                        switch (uiSelectionCaller) {
                            case CallerTypes.Candidate:
                                {
                                    return (

                                        <Grid2 container>
                                            <CandidateDetailCharts key="candidateDetailsCharts" />
                                        </Grid2>
                                    )
                                    break;
                                }

                            case CallerTypes.CardOverview:
                                {
                                    return (

                                        <Grid2 container key="cardLoaderGrid">
                                            <CandidateCardLoaderParent key="cardLoaderParent">

                                                <Grid2 container>
                                                    <CandidateCardLoaderChild />
                                                </Grid2>

                                            </CandidateCardLoaderParent>
                                        </Grid2>

                                    )

                                    break;
                                }
                            default:
                                {
                                    return (
                                        <p>Content Loading. Please Wait
                                        </p>
                                    );
                                    break;
                                }
                        }
                    })()}

                </Grid2>

            </Grid2>
        </Box>
        
    );
}