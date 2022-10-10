

import { createSlice } from '@reduxjs/toolkit'
import { RootState } from './store';

//maintains list of children in UI container component (such as drop down list)
export const uiContainerSlice = createSlice({
    name: 'children',
    initialState: {
        children:[],
        

    },
    reducers: {
        update: (state, action) => {
            state.children = action.payload;
            
        }


    }

})


export const selectChildren = (state: RootState) => state.uiContainer.children;
export const { update } = uiContainerSlice.actions

export default uiContainerSlice.reducer