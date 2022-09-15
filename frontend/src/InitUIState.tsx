import React, { useState } from 'react';
//import axios from 'axios';
import * as MDWatchAPI from './MDWatchAPI'

export default function InitUIState() {
    //set initial app state by retriving UI and card data from API
    
    const clientConfig = new MDWatchAPI.Configuration()
    clientConfig.basePath = "http://localhost:4998";
    const client = new MDWatchAPI.UIApi(clientConfig);
    const [uiResult, setUIResult] = React.useState<MDWatchAPI.CandidateUIDTO[]>([]);
    
    const getData = async () => {
        setUIResult((await client.apiUICandidatesbyYearYearGet(2022, true)).data);
    }
    React.useEffect(() => {
        getData();
    }, );
    console.log(uiResult);
    return uiResult;   
}