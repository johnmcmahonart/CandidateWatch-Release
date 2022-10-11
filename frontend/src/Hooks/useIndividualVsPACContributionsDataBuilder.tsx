import React from 'react';
import CircularProgress from "@mui/material/CircularProgress";
import { useSelector } from "react-redux";
import { FinanceTotalsDto, useGetApiFinanceTotalsByKeyYearsQuery } from "../APIClient/MDWatch";
import { IChartData } from "../Interfaces/Components";
import PrepareLabelsforDisplay from "../PrepareLabels";

import { selectData } from "../Redux/UISelection";
import { ValidateToken } from "../Utilities";
import { useElectionCycle } from "./useElectionCycle"
export default function useTop5CandidateDonationDataBuilder(): IChartData[] | JSX.Element {
    const uiSelectionData = useSelector(selectData);

    const buildFinanceData = (source: FinanceTotalsDto): IChartData[] => {
        const outData: Array<IChartData> = [];
        outData.push({
            label: "Indivudal Contributions",
            dataKey: ValidateToken(source.individualItemizedContributions || 0),
            labelShort: "Indivudal Contributions",
            toolTipItemLabel: "Indivudal Contributions",
            toolTipItemValueLabel: "Dollars"
        })
        outData.push({
            label: "Non-Indivudal Contributions",
            dataKey: ValidateToken(source.otherPoliticalCommitteeContributions || 0),
            labelShort: "Non-Indivudal Contributions",
            toolTipItemLabel: "Non-Indivudal Contributions",
            toolTipItemValueLabel: "Dollars"
        })
        return PrepareLabelsforDisplay(outData, false, 10);
    }

    let data: any = useElectionCycle(uiSelectionData.candidateId, uiSelectionData.electionYear);
    if (useElectionCycle(uiSelectionData.candidateId, uiSelectionData.electionYear) != false) {
        return buildFinanceData(data);
    }

    else {
        return (

            <CircularProgress />
        )
    }
}