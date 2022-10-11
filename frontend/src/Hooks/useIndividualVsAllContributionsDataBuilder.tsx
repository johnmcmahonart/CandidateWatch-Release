import React from 'react';
import CircularProgress from "@mui/material/CircularProgress";
import { useSelector } from "react-redux";
import { FinanceTotalsDto, ScheduleBDetailDto, useGetApiFinanceTotalsByKeyYearsQuery, useGetApiFinanceTotalsByYearKeysQuery, useGetApiFinanceTotalsKeysQuery, useGetApiScheduleBDetailByKeyYearsQuery, useGetApiScheduleBDetailByYearKeysQuery, useGetApiScheduleBOverviewKeysQuery, useGetApiUiCandidatesbyYearByYearQuery } from "../APIClient/MDWatch";
import { IChartData } from "../Interfaces/Components";
import PrepareLabelsforDisplay from "../PrepareLabels";
import { selectChildren } from "../Redux/UIContainer";
import { selectData } from "../Redux/UISelection";
import { ValidateToken } from "../Utilities";
import { useElectionCycle } from './useElectionCycle';

export default function useIndividualVsAllContributionsDataBuilder(): IChartData[] | JSX.Element {
    const uiSelectionData = useSelector(selectData);
    const uiContainerData = useSelector(selectChildren);

    const contributionTotals: IChartData[] = [];

    const buildFinanceData = (source: number[]): IChartData[] => {
        const outData: Array<IChartData> = [];
        outData.push({
            label: "State Total Contributions",
            dataKey: ValidateToken(source[0] || 0),
            labelShort: "State Total",
            toolTipItemLabel: "State Total",
            toolTipItemValueLabel: "Dollars"
        })
        outData.push({
            label: "Indivudal Total Contributions",
            dataKey: ValidateToken(source[1] || 0),
            labelShort: "Indivudal Total Contributions",
            toolTipItemLabel: "Indivudal Total Contributions",
            toolTipItemValueLabel: "Dollars"
        })
        return PrepareLabelsforDisplay(outData, false, 10);
    }
    //get data for House members
    const financeDataHouse = useGetApiFinanceTotalsByYearKeysQuery({ keys: uiContainerData.where((c: string) => c.startsWith("H")).toArray(), year: uiSelectionData.electionYear })

    const senateCandidates = uiContainerData.where((c: string) => c.startsWith("S")).toArray();

    //get correct data based on cycle

    let senatorTotals: Map<string, number> = new Map();
    let senateDataA: any = useElectionCycle(senateCandidates[0], uiSelectionData.electionYear);

    let senateDataB: any = useElectionCycle(senateCandidates[1], uiSelectionData.electionYear);
    if (senateDataA != false) {
        let total = ValidateToken<number>(senateDataA.individualItemizedContributions || 0) + ValidateToken<number>(senateDataA.otherPoliticalCommitteeContributions || 0)
        senatorTotals.set(senateCandidates[0], total)
    }
    if (senateDataB != false) {
        let total = ValidateToken<number>(senateDataB.individualItemizedContributions || 0) + ValidateToken<number>(senateDataB.otherPoliticalCommitteeContributions || 0)
        senatorTotals.set(senateCandidates[1], total)
    }

    if (financeDataHouse.isSuccess) {
        //add senate data to house data

        //calculate state vs candidate total
        const contributionTotals: number[] = []
        let stateTotal: number = 0;
        financeDataHouse.data[0].forEach((candidateFinance: FinanceTotalsDto) => {
            stateTotal += ValidateToken<number>(candidateFinance.individualItemizedContributions || 0) + ValidateToken<number>(candidateFinance.otherPoliticalCommitteeContributions || 0);
        })
        for (const total of senatorTotals.values()) {
            stateTotal += total;
        }
        //check if current selected candidate is member of house or senate
        let candidateTotal: number = 0;
        if (uiSelectionData.candidateId.startsWith("H")) {
            const candidateHouseFinanceData: FinanceTotalsDto[] = financeDataHouse.data[0].where((c => c.candidateId === uiSelectionData.candidateId)).toArray();

            if (candidateHouseFinanceData.length > 0) {
                candidateTotal = ValidateToken<number>(candidateHouseFinanceData[0].individualItemizedContributions || 0) + ValidateToken<number>(candidateHouseFinanceData[0].otherPoliticalCommitteeContributions || 0)
            }
        }
        else {
            candidateTotal = ValidateToken<number>(senatorTotals.get(uiSelectionData.candidateId) || 0);
        }

        contributionTotals.push(stateTotal);
        contributionTotals.push(candidateTotal);

        return buildFinanceData(contributionTotals);
    }

    else {
        return (

            <CircularProgress />
        )
    }
}