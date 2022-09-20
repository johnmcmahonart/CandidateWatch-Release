import * as MDWatchAPI from '../MDWatchAPI'
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText'
import { forEachChild } from 'typescript';
import { element, string } from 'prop-types';
import { ICandidateCard } from '../Interfaces/Components';
import CandidateCard from './CandidateCard';

//maps DTO to react component
export default function CandidateCardBuilder(candidateProp: MDWatchAPI.CandidateUIDTO[], yearProp:number) {
    const builtNavElements: Array<JSX.Element> = [];

    candidateProp.forEach(element => {
        let builtElement: ICandidateCard = {
            
                fullName: element.firstName+" "+element.lastName,
            district: typeof(string)element.district,
            party: element.party,
            electionYear:yearProp,
            moreDetail: ""
            
        };
        builtNavElements.push(CandidateCard(builtElement));
    });



    return builtNavElements
