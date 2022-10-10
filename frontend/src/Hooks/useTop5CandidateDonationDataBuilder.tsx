import React from 'react';
import CircularProgress from "@mui/material/CircularProgress";
import { useSelector } from "react-redux";
import { ScheduleBDetailDto, useGetApiScheduleBDetailByKeyYearsQuery, useGetApiScheduleBDetailByYearKeysQuery, useGetApiScheduleBOverviewKeysQuery, useGetApiUiCandidatesbyYearByYearQuery } from "../APIClient/MDWatch";
import { IChartData } from "../Interfaces/Components";
import PrepareLabelsforDisplay from "../PrepareLabels";
import { selectChildren } from "../Redux/UIContainer";
import { selectData } from "../Redux/UISelection";
import { ValidateToken } from "../Utilities";

export default function useTop5CandidateDonationDataBuilder(): IChartData[] | JSX.Element {
    //get all ScheduleBData for all candidates in uiContainer, so we can determine who are the top5 in donations from PACs
    const uiContainerData = useSelector(selectChildren);
    const uiSelectionData = useSelector(selectData);

    const scheduleBData = new Map();

    //uiData contains full name, which we need. We also need scheduleB overview data to join candidateId to recipientId. recipientId is used as key scheduleBDetail data
    const uiData = useGetApiUiCandidatesbyYearByYearQuery({ wasElected: true, year: uiSelectionData.electionYear });
    let overviewDataFix: string = "keys=";
    let i = 0;
    uiContainerData.forEach((e) => {
        i++;
        if (i != uiContainerData.length - 1) {
            overviewDataFix += "e" + "&"
        }
        else {
            overviewDataFix += "e"
        }
            
    })

    const scheduleBOverviewData = useGetApiScheduleBOverviewKeysQuery({ keys: uiContainerData });
    const candidateScheduleBData = useGetApiScheduleBDetailByYearKeysQuery({ keys: uiContainerData, year: uiSelectionData.electionYear })
    if (uiData.isSuccess && scheduleBOverviewData.isSuccess && candidateScheduleBData.isSuccess) {
        
        candidateScheduleBData.data.map((candidateScheduleBDetail: ScheduleBDetailDto[]) => {
            let sum = 0
            candidateScheduleBDetail.forEach((i: ScheduleBDetailDto) => {
                sum += ValidateToken<number>(i.total || 0)
                let { candidateId } = scheduleBOverviewData.data[0].where(s => s.principalCommitteeId === i.recipientId).first()
                let { firstName, lastName } = uiData.data.where(c => c.candidateId === candidateId).first();
                scheduleBData.set(firstName + " " + lastName, sum);
            })
        })
        const buildTop5CandidateData = (source: Map<string, number>): IChartData[] => {
            const mappedData: Array<IChartData> = [];

            for (let entry of source.entries()) {
                mappedData.push({
                    label: ValidateToken<string>(entry[0] || ""),
                    dataKey: entry[1],
                    labelShort: entry[0],
                    toolTipItemLabel: "Candidate",
                    toolTipItemValueLabel: "Dollars"
                })
            }

            return PrepareLabelsforDisplay(mappedData.orderBy((x => x.dataKey)).reverse().take(5).toArray(), true, 10);
        }

        return buildTop5CandidateData(scheduleBData)
    }

    else {
        return (

            <CircularProgress />
        )
    }
}