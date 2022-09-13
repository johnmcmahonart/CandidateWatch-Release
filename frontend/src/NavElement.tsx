import React from 'react';
import List from '@mui/material/List';
import { INavElement } from '../Interfaces/UI';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText'

function NavElement(element: INavElement) {
    return (
        //<ListItemButton onClick="">
        <ListItemButton key={element.label}>
            <ListItemText primary={element.text} />

        </ListItemButton>

    );
}

export default NavElement;