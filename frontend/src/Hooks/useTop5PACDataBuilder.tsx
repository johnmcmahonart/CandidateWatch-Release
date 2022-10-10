import CircularProgress from "@mui/material/CircularProgress";
import { useSelector } from "react-redux";
import { ScheduleBDetailDto, useGetApiScheduleBDetailByKeyYearsQuery } from "../APIClient/MDWatch";
import React from 'react';
import { IChartData } from "../Interfaces/Components";
import PrepareLabelsforDisplay from "../PrepareLabels";
import { selectData } from "../Redux/UISelection";
import { ValidateToken } from "../Utilities";
export default function useTop5PACDataBuilder() :IChartData[]|JSX.Element
    {
    const uiSelectionData = useSelector(selectData);
    const scheduleBData = useGetApiScheduleBDetailByKeyYearsQuery({ key: uiSelectionData.candidateId, years: [uiSelectionData.electionYear] });
    const buildTop5ChartData = (source: any): IChartData[] => {
        const mappedData: Array<IChartData> = [];

        source.data.map((element: ScheduleBDetailDto) => (
            mappedData.push({
                label: ValidateToken<string>(element.committeeName||""),
                dataKey: element.total,
                labelShort: ValidateToken<string>(element.committeeName||""),
                toolTipItemLabel: "PAC Name",
                toolTipItemValueLabel: "Dollars"
            })
        ))

        const chartData: Array<IChartData> = PrepareLabelsforDisplay(mappedData.orderBy((x => x.dataKey)).reverse().take(5).toArray(), true, 10);

        return chartData;
    }
    if (scheduleBData.isSuccess) {
        return buildTop5ChartData(scheduleBData);
    }
    
    return (
        <CircularProgress/>
        )
}