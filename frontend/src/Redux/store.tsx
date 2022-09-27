import { configureStore } from '@reduxjs/toolkit';
import { uiSelectionSlice } from './UISelection';
import { MDWatchAPI } from '../APIClient/MDWatch'
export const store = configureStore({
    reducer: {
        uiSelection: uiSelectionSlice.reducer,
        [MDWatchAPI.reducerPath]:MDWatchAPI.reducer
    },
    middleware:getDefaultMiddleware => getDefaultMiddleware().concat(MDWatchAPI.middleware)
})
// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>
// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch