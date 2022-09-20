import React from 'react';
import logo from './logo.svg';
import './App.css';
import Container from '@mui/material/Container';
import Grid from '@mui/material/Unstable_Grid2';
import Paper from '@mui/material/Paper';
import { styled } from '@mui/material/styles';
import  InitUIState from './APIClient/InitUIState';
import { string } from 'prop-types';
import response from 'axios';
import * as MDWatchAPI from './MDWatchAPI'
import NavListBuilder from './Components/NavListBuilder'
import NavList from './Components/NavList';
const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
    ...theme.typography.body2,
    padding: theme.spacing(1),
    textAlign: 'center',
    color: theme.palette.text.secondary,
}));

function App() {
    
    let defaultElectedListChildren:Array<JSX.Element>=[];
   
    defaultElectedListChildren = NavListBuilder(InitUIState());
    
    return (
        <Container className="master" maxWidth="xl">
            <Grid container spacing={1} columns={10}>
                <Grid className="navigation" xs={2}>
                    <Item>
                        <NavList>{defaultElectedListChildren}</NavList>
                    </Item>
                    

                </Grid>
                <Grid className="mainContent" xs={8}>
                    <Item>main content test</Item>
                </Grid>
            </Grid>
        </Container>
    );
}

export default App;