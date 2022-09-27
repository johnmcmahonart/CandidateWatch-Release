import React from 'react';
import logo from './logo.svg';
import '../App.css';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import { ExpandLess, ExpandMore } from '@mui/icons-material';
import Collapse from '@mui/material/Collapse/Collapse';
export default function NavList({ children }: { children: React.ReactNode | React.ReactNode[] } ) {
    const [open, setOpen] = React.useState(false);

    const handleClick = () => {
        setOpen(!open);
    };
    return (
        <List sx={{ width: '100%' }}>
            <ListItemButton onClick={handleClick}>
                
                <ListItemText primary="Federally Elected Officials" />
                {open ? <ExpandLess /> : <ExpandMore />}
            </ListItemButton>
            <Collapse in={open} timeout="auto" unmountOnExit>
                <List component="div" disablePadding>
                    {children }
                </List>
            </Collapse>




        </List>
        );
}

