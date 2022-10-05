import React from 'react';

import './App.css';

import Grid2 from '@mui/material/Unstable_Grid2';
import Paper from '@mui/material/Paper';
import { styled } from '@mui/material/styles';

import NavListChildren from './Components/NavListChildren'
import NavList from './Components/NavList';
import MainContentContainer from './Components/MainContentContainer';
import { IEnumerable, initializeLinq } from 'linq-to-typescript';
import AppBar from '@mui/material/AppBar';

const Item = styled(Paper)(({ theme }) => ({
    backgroundColor: theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
    ...theme.typography.body2,
    padding: theme.spacing(1),
    textAlign: 'center',
    color: theme.palette.text.secondary,
}));
//enable linq across app to work on arrays and maps
declare global {
    interface Array<T> extends IEnumerable<T> { }
    interface Uint8Array extends IEnumerable<number> { }
    interface Uint8ClampedArray extends IEnumerable<number> { }
    interface Uint16Array extends IEnumerable<number> { }
    interface Uint32Array extends IEnumerable<number> { }
    interface Int8Array extends IEnumerable<number> { }
    interface Int16Array extends IEnumerable<number> { }
    interface Int32Array extends IEnumerable<number> { }
    interface Float32Array extends IEnumerable<number> { }
    interface Float64Array extends IEnumerable<number> { }
    interface Map<K, V> extends IEnumerable<[K, V]> { }
    interface Set<T> extends IEnumerable<T> { }
    interface String extends IEnumerable<string> { }
}

initializeLinq();


function App() {
    return (

        <Grid2 container spacing={0} columns={10}>

            <Grid2 className="navigation" xs={10} sm={2}>

                <NavList>
                    <NavListChildren />
                </NavList>

            </Grid2>
            
            <Grid2 xs={10} sm={8}>
                <AppBar position="static">
                    <p style={{ textAlign: 'center' }}>Breadcrumb and filter</p>
                </AppBar>
                <MainContentContainer />

            </Grid2>

        </Grid2>

    );
}

export default App;