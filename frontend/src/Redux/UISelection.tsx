import { createSlice } from '@reduxjs/toolkit'

export const uiSelectionSlice = createSlice({
    name: 'selection',
    initialState: {
        data: "none",
        callerType:"none"
    },
    reducers: {
        update: (state, action) => {
            state.data = action.payload[0];
            state.callerType = action.payload[1];
        }
        
        
    }
    
})


export const selectData = (state: { data: string; }) => state.data;
export const selectCaller = (state: { callerType: string; }) => state.callerType;
export const { update } = uiSelectionSlice.actions

export default uiSelectionSlice.reducer