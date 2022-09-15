import React, { Fragment } from 'react';
import List from '@mui/material/List';
import { INavElement } from '../Interfaces/UI';
import * as MDWatchAPI from './MDWatchAPI'
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText'
import { forEachChild } from 'typescript';
import { element, string } from 'prop-types';
import NavElement from './NavElement';

export default function NavListBuilder(props: MDWatchAPI.CandidateUIDTO[]) {
    const navElements: Array<INavElement> = [];
    props.forEach(element => {
        const t = {
            label: element.candidateId ? (element.candidateId) : "",
            text: element.firstName + " " + element.lastName,
            onClick: false
        };
        navElements.push(t);
    });
    
    const builtElements: Array<JSX.Element> = [];
    navElements.forEach(item => builtElements.push(NavElement(item)));
    return builtElements
}