import React from 'react';
import { useGetApiUiCandidatesbyYearByYearQuery } from '../APIClient/MDWatch';
import CandidateCardLoader from './CandidateCardLoader';
import { nanoid } from 'nanoid';
import Grid2 from '@mui/material/Unstable_Grid2';
export default function CandidateCardLoaderChild():JSX.Element {
    const candidatesData = useGetApiUiCandidatesbyYearByYearQuery({ year: 2022, wasElected: true });

    //first get just the candidate ID from the ui endpoint for the year, then get detailed data for each candidate

    if (candidatesData.isSuccess) {
        return <>
            
                {
                    candidatesData.data.map((uiData => (<CandidateCardLoader key={nanoid()}

                        uiData={uiData}
                    />
                    )))

                }
            
</>
    }
    return <>
        <div>
        </div>
    </>           

                        
    }



