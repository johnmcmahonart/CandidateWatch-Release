import React from 'react';
import { useSelector } from 'react-redux';
import { RootState } from '../Redux/store';
import { selectData } from '../Redux/UISelection';
export default function CandidateDetail() {
    const uiSelectionData = useSelector(selectData);
    return (
        <p>Candidate name is: {uiSelectionData }</p>
        )

}