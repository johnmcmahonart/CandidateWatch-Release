import React from "react";
import { CandidateUidto } from "../../APIClient/MDWatch";


export interface ICandidateCard {
    fullName: string|null|undefined;
    district: string | null |undefined;
    party: string | null | undefined;
    electionYear: number;
    moreDetail: string | null | undefined;


}

export interface ICardLoader {
    uiData: CandidateUidto;
}
export interface ILineChartData {
    
    xAxisLabel: string | null | undefined;
    dataKey:any;

}