import React from 'react';
import List from '@mui/material/List';
import { INavElement } from '../Interfaces/Menu';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText'

export default function NavElement(element: INavElement) {
    
    return (
        //<ListItemButton onClick="">
        <ListItemButton key={element.label}>
            <ListItemText primary={element.text} />

        </ListItemButton>

    );
}

