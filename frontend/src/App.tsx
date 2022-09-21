import React from 'react';
import logo from './logo.svg';
import './App.css';
import Container from '@mui/material/Container';
import Grid2 from '@mui/material/Unstable_Grid2';
import Paper from '@mui/material/Paper';
import { styled } from '@mui/material/styles';
import  InitUIState from './APIClient/InitUIState';
import { string } from 'prop-types';
import response from 'axios';
import * as MDWatchAPI from './MDWatchAPI'
import NavListBuilder from './Components/NavListBuilder'
import NavList from './Components/NavList';
import CandidateCardBuilder from './Components/CandidateCardBuilder';
import MainContentContainer from './Components/MainContentContainer';

const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
    ...theme.typography.body2,
    padding: theme.spacing(1),
    textAlign: 'center',
    color: theme.palette.text.secondary,
}));

function App() {
    
    let defaultElectedListChildren:Array<JSX.Element>=[];
    let initialCandidateCards:Array<React.ReactNode>= [];
    defaultElectedListChildren = NavListBuilder(InitUIState());
    initialCandidateCards = CandidateCardBuilder(InitUIState(), 2022);
    
    return (
        
            <Grid2 container spacing={1} columns={10}>
            <Grid2 className="navigation" xs={10} lg={1.2}>
                    <Item>
                        <NavList>{defaultElectedListChildren}</NavList>
                    </Item>
                    

                </Grid2>
                <Grid2 className="mainContent" xs={8}>
                    <MainContentContainer>
                        {initialCandidateCards}
                    </MainContentContainer>

                </Grid2>
            </Grid2>
        
    );
}

export default App;