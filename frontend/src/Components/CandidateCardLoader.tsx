import React from 'react';
import { useGetApiCandidateByKeyQuery } from "../APIClient/MDWatch";
import { ICardLoader } from "../Interfaces/Components";
import CandidateCard from "./CandidateCard";


//loads card detail data for candidate as provided from CandidateCardBuilder. this is needed since endpoint queries need to be chained
export default function CandidateCardLoaderInner(props: ICardLoader) {
    let key = ""
    if (props.uiData.candidateId) {
        key = props.uiData.candidateId;
    }
    const candidatesDetailData = useGetApiCandidateByKeyQuery({ key: key })
    if (candidatesDetailData.isSuccess) {
        return <>
            {
                candidatesDetailData.data.map((cardData => (<CandidateCard key={props.uiData.candidateId}
                    fullName={props.uiData.firstName + " " + props.uiData.lastName}
                    district={cardData.district}
                    party={cardData.party}
                    electionYear={2022}
                    moreDetail=""
                />
                )))
            }

        </>
    }
    return <>
<div></div>
    </>
}