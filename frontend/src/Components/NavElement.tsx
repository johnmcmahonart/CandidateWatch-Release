import React, { useState } from 'react';
import { INavElement } from '../Interfaces/Menu';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import {nanoid } from 'nanoid'
import { useDispatch } from 'react-redux';
import { update } from '../Redux/UISelection';
export default function NavElement(element: INavElement) {
    const dispatch = useDispatch();

    const setCandidateId= () => {
        dispatch(update([element.candidateId,"caller was candidate selector"]))
        
    };
    
    return (
        <ListItemButton key={nanoid()} onClick={setCandidateId} >
            <ListItemText primary={element.text} />

        </ListItemButton>

    );
}

