import React from 'react';
import { useSelector } from 'react-redux';
import logo from './logo.svg';
import '../App.css';
import List from '@mui/material/List';
import InboxIcon from '@mui/icons-material/MoveToInbox'
import NavElement from './NavElement'
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import { ExpandLess, ExpandMore } from '@mui/icons-material';
import Collapse from '@mui/material/Collapse/Collapse';
import Grid2 from '@mui/material/Unstable_Grid2';
import { selectData, selectCaller } from '../Redux/UISelection';
function MainContentContainer({ children }: { children: React.ReactNode | React.ReactNode[] }) {
    
    const [open, setOpen] = React.useState(false);
    const uiSelectionData = useSelector(selectData);
    const uiSelectionCaller = useSelector(selectCaller);
    //console.log(candidateId);
    //wrapper for MainContent area, so the children can be swapped out when the user selects a navigation item
    return (
        
        <Grid2 container spacing={2}>
            {uiSelectionCaller}
            {uiSelectionData }
        </Grid2>
    );
}

export default MainContentContainer