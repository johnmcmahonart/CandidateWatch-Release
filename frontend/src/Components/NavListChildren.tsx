import React, { Component } from 'react';



import NavElement from './NavElement';

import { CandidateUidto, useGetApiUiCandidatesbyYearByYearQuery } from '../APIClient/MDWatch';
import { update } from '../Redux/UIContainer';
import { useDispatch } from 'react-redux';
import { ValidateToken } from '../Utilities';

//build NavElements from data from ui endpoint
export default function NavListChildren(): JSX.Element {
    const dispatch = useDispatch();

    


    const uiData = useGetApiUiCandidatesbyYearByYearQuery({ year: 2022, wasElected: true, state:"MD" });

    //return react fragments once data is loaded
    if (uiData.isSuccess) {

        //get the candidateIds for all the child elements and store them in redux store
        let children: string[] = []
        uiData.data.map((element: CandidateUidto) => (
            children.push(ValidateToken<string>(element.candidateId||""))
        ))
        dispatch(update(children));
        return <>
            {
                uiData.data.map((uiElement => (<NavElement key={uiElement.candidateId }
                    
                    text={uiElement.firstName + ' ' + uiElement.lastName}
                    candidateId={uiElement.candidateId} />
                )))

            }</>
    }
    return <>
        <div>
        </div>
    </>
}