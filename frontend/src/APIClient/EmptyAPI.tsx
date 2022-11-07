// Or from '@reduxjs/toolkit/query' if not using the auto-generated hooks
import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'

// initialize an empty api service that we'll inject endpoints into later as needed

//configure endpoint based on build enviornment
const baseURL = process.env.REACT_APP_API_URL
console.log(baseURL);
export const emptySplitApi = createApi({
    baseQuery: fetchBaseQuery({ baseUrl: baseURL }),
    endpoints: () => ({}),
})