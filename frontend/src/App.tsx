import React from 'react';

import './App.css';

import Grid2 from '@mui/material/Unstable_Grid2';
import Paper from '@mui/material/Paper';
import { styled } from '@mui/material/styles';

import NavListChildren from './Components/NavListChildren'
import NavList from './Components/NavList';
import MainContentContainer from './Components/MainContentContainer';

const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
    ...theme.typography.body2,
    padding: theme.spacing(1),
    textAlign: 'center',
    color: theme.palette.text.secondary,
}));

function App() {
    return (

        <Grid2 container spacing={0} columns={10}>

            <Grid2 className="navigation" xs={10} sm={2}>
                
                    <NavList>
                        <NavListChildren />
                    </NavList>
                
            </Grid2>
            <Grid2 xs={10} sm={8}>
                
                    <MainContentContainer />
                

            </Grid2>

        </Grid2>

    );
}

export default App;