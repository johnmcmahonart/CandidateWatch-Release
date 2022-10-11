import CircularProgress from "@mui/material/CircularProgress";
import { useSelector } from "react-redux";
import { ScheduleBDetailDto, useGetApiScheduleBDetailByKeyYearsQuery } from "../APIClient/MDWatch";
import React from 'react';
import { IChartData } from "../Interfaces/Components";
import PrepareLabelsforDisplay from "../PrepareLabels";
import { selectData } from "../Redux/UISelection";
import { ValidateToken } from "../Utilities";
import { NumberComparer } from "linq-to-typescript";
export default function useTop5PACDataBuilder() :IChartData[]|JSX.Element
    {
    const uiSelectionData = useSelector(selectData);
    const scheduleBData = useGetApiScheduleBDetailByKeyYearsQuery({ key: uiSelectionData.candidateId, years: [uiSelectionData.electionYear] });
    const buildTop5ChartData = (source: any): IChartData[] => {
        const mappedData: Array<IChartData> = [];

        source.data.map((element: ScheduleBDetailDto) => (
            mappedData.push({
                label: ValidateToken<string>(element.committeeName||""),
                dataKey: ValidateToken<number>(element.total||0),
                labelShort: ValidateToken<string>(element.committeeName||""),
                toolTipItemLabel: "PAC Name",
                toolTipItemValueLabel: "Dollars"
            })
        ))

        

        return PrepareLabelsforDisplay(mappedData.orderBy((x: IChartData) => x.dataKey, NumberComparer).reverse().take(5).toArray(),true, 10);
    }
    if (scheduleBData.isSuccess) {
        return buildTop5ChartData(scheduleBData);
    }
    
    return (
        <CircularProgress/>
        )
}