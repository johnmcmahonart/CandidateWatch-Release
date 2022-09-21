import React, { Fragment } from 'react';
import List from '@mui/material/List';
import { INavElement } from '../Interfaces/Menu';

import * as MDWatchAPI from '../MDWatchAPI'
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText'
import { forEachChild } from 'typescript';
import { element, string } from 'prop-types';
import NavElement from './NavElement';

//maps DTO to react component
export default function NavListBuilder(props: MDWatchAPI.CandidateUIDTO[]) {
    const builtNavElements: Array<JSX.Element> = [];
    
    props.forEach(element => {
        let builtElement:INavElement= {
            label: element.candidateId,
            text: element.firstName + " " + element.lastName,
            onClick: false
        };
        builtNavElements.push(NavElement(builtElement));
    });
    
    
    
    return builtNavElements
}