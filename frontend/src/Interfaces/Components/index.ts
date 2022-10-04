import React from "react";
import { CandidateUidto } from "../../APIClient/MDWatch";

export interface ICandidateCard {
    fullName: string | null | undefined;
    district: string | null | undefined;
    party: string | null | undefined;
    electionYear: number;
    moreDetail: string | null | undefined;
}

export interface ICardLoader {
    uiData: CandidateUidto;
}
export interface IChartData {
    label: string;
    labelShort: string;
    dataKey: any;
    xAxisLabel: string
    yAxisLabel: string
}
export interface ILineChart {
    xAxisLabel: string
    yAxisLabel: string
    data: IChartData[]
}

export interface IChartToolTip {
    xAxisLabelFull: string
    xAxisValue: string
    yAxisLabelFull: string
    yAxisValue: string
}