import React from 'react';
import { useSelector } from 'react-redux';
import logo from './logo.svg';
import '../App.css';
import Grid2 from '@mui/material/Unstable_Grid2';
import { selectData, selectCaller } from '../Redux/UISelection';
import { CallerTypes } from '../Enums';
import CandidateDetail from './CandidateDetail';
import type { RootState, AppDispatch } from '../Redux/store'
import CandidateCardLoader from './CandidateCardLoader';
import CandidateCardLoaderParent from './CandidateCardLoaderParent';
import CandidateCardLoaderChild from './CandidateCardLoaderChild';

export default function MainContentContainer() {
    const [open, setOpen] = React.useState(false);

    const uiSelectionCaller = useSelector(selectCaller);

    //wrapper for MainContent area, so the children can be swapped out when the user selects a navigation item
    return (
        <Grid2 container spacing={1} columns={8}>
            <Grid2 xs={8} key="gridSwitch">

                {(function () {
                    switch (uiSelectionCaller) {
                        case CallerTypes.Candidate:
                            {
                                return (

                                    <CandidateDetail key="candidateDetailsCharts" />

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
    );
}