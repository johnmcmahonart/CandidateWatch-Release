import React, { useState } from 'react';
import { INavElement } from '../Interfaces/Menu';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import {nanoid } from 'nanoid'
import { useDispatch } from 'react-redux';
import { update } from '../Redux/UISelection';

import { CallerTypes } from '../Enums'
export default function NavElement(props: INavElement) {
    const dispatch = useDispatch();

    const setCandidateId= () => {
        dispatch(update([{ electionYear: 2022,candidateId:props.candidateId },CallerTypes.Candidate]))
        
    };
    
    return (
        <ListItemButton key={nanoid()} onClick={setCandidateId} >
            <ListItemText primary={props.text} />

        </ListItemButton>

    );
}

