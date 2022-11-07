import React from 'react';
import CircularProgress from "@mui/material/CircularProgress";
import { useSelector } from "react-redux";
import { ScheduleBDetailDto, useGetApiScheduleBDetailByKeyYearsQuery, useGetApiScheduleBDetailByYearKeysQuery, useGetApiScheduleBOverviewKeysQuery, useGetApiUiCandidatesbyYearByYearQuery } from "../APIClient/MDWatch";
import { IChartData } from "../Interfaces/Components";
import PrepareLabelsforDisplay from "../PrepareLabels";
import { selectChildren } from "../Redux/UIContainer";
import { selectData } from "../Redux/UISelection";
import { ValidateToken } from "../Utilities";
import { from, NumberComparer } from 'linq-to-typescript';

export default function useTop5CandidateDonationDataBuilder(): IChartData[] | JSX.Element {
    //get all ScheduleBData for all candidates in uiContainer, so we can determine who are the top5 in donations from PACs
    const uiContainerData = useSelector(selectChildren);
    const uiSelectionData = useSelector(selectData);

    const scheduleBData: Map<string, number> = new Map<string, number>;

    //uiData contains full name, which we need. We also need scheduleB overview data to join candidateId to recipientId. recipientId is used as key scheduleBDetail data
    const uiData = useGetApiUiCandidatesbyYearByYearQuery({ wasElected: true, year: uiSelectionData.electionYear, state:"MD" });

    const buildTop5CandidateData = (source: Map<string, number>): IChartData[] => {
        const mappedData: Array<IChartData> = [];

        for (let entry of source.entries()) {
            mappedData.push({
                label: ValidateToken<string>(entry[0] || ""),
                dataKey: ValidateToken<number>(entry[1] || 0),
                labelShort: entry[0],
                toolTipItemLabel: "Candidate",
                toolTipItemValueLabel: "Dollars"
            })
        }

        return PrepareLabelsforDisplay(mappedData.orderBy((x: IChartData) => x.dataKey, NumberComparer).reverse().take(5).toArray(),true, 10);
    }
    const scheduleBOverviewData = useGetApiScheduleBOverviewKeysQuery({ keys: uiContainerData, state:"MD" });
    const candidateScheduleBData = useGetApiScheduleBDetailByYearKeysQuery({ keys: uiContainerData, year: uiSelectionData.electionYear, state:"MD" })
    if (uiData.isSuccess && scheduleBOverviewData.isSuccess && candidateScheduleBData.isSuccess) {
        candidateScheduleBData.data.map((candidateScheduleBDetail: ScheduleBDetailDto[]) => {
            let sum: number = 0
            candidateScheduleBDetail.forEach((i: ScheduleBDetailDto) => {
                sum += ValidateToken<number>(i.total || 0)
                let { candidateId } = scheduleBOverviewData.data[0].where(s => s.principalCommitteeId === i.recipientId).first()
                let { firstName, lastName } = uiData.data.where(c => c.candidateId === candidateId).first();
                scheduleBData.set(firstName + " " + lastName, sum);
            })
        })

        return buildTop5CandidateData(scheduleBData)
    }

    else {
        return (

            <CircularProgress />
        )
    }
}