import React from 'react';
import Get from 'axios';



const apiBase = 'http://localhost:5018/api'
export default  async function InitUIState() {
  //set initial app state by retriving UI and card data from API  
    
    const axios = require('axios');
    await React.useEffect(() => {
        axios({
            method: 'get',
            url: apiBase+'/UI/CandidatesbyYear/2022',
            data: {
                wasElected: true
            }

        });
        axios.get().then(Response);


    })
    

    return (axios.Response.data); 
        
    
}

