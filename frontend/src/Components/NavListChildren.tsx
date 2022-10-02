import React, { Component } from 'react';



import NavElement from './NavElement';

import { useGetApiUiCandidatesbyYearByYearQuery } from '../APIClient/MDWatch';

//build NavElements from data from ui endpoint
export default function NavListChildren(): JSX.Element {
    const uiData = useGetApiUiCandidatesbyYearByYearQuery({ year: 2022, wasElected: true });

    //return react fragments once data is loaded
    if (uiData.isSuccess) {
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