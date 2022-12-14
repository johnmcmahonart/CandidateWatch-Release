import {CallerTypes } from '../Enums'
import { createSlice } from '@reduxjs/toolkit'
import { RootState } from './store';
import {IUIData } from '../Interfaces/Menu'
export const uiSelectionSlice = createSlice({
    name: 'selection',
    initialState:  {
        data: {electionYear:2022, candidateId:""},
        callerType:CallerTypes.CardOverview 

    },
    reducers: {
        update: (state, action) => {
            state.data = action.payload[0];
            state.callerType = action.payload[1];
        }
        
        
    }
    
})


export const selectData = (state:RootState) => state.uiSelection.data;
export const selectCaller = (state:RootState) => state.uiSelection.callerType;
export const { update } = uiSelectionSlice.actions

export default uiSelectionSlice.reducer