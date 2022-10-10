import React from 'react';
import CircularProgress from "@mui/material/CircularProgress";
import { useSelector } from "react-redux";
import { FinanceTotalsDto, ScheduleBDetailDto, useGetApiFinanceTotalsByKeyYearsQuery, useGetApiScheduleBDetailByKeyYearsQuery, useGetApiScheduleBDetailByYearKeysQuery, useGetApiScheduleBOverviewKeysQuery, useGetApiUiCandidatesbyYearByYearQuery } from "../APIClient/MDWatch";
import { IChartData } from "../Interfaces/Components";
import PrepareLabelsforDisplay from "../PrepareLabels";
import { selectChildren } from "../Redux/UIContainer";
import { selectData } from "../Redux/UISelection";
import { ValidateToken } from "../Utilities";

export default function useTop5CandidateDonationDataBuilder(): IChartData[] | JSX.Element {
    const uiSelectionData = useSelector(selectData);
    
    //senators alternate election cycles, and there isn't a way to know within data contained in FEC API which cycle a senator was elected in,
    //if no data is returned for a given cycle, we assume they were elected in the other 6 year cycle (current even numbered year +2)
    let data: IChartData;
    let financeDataCycleA = useGetApiFinanceTotalsByKeyYearsQuery({ key: uiSelectionData.candidateId, years: [uiSelectionData.electionYear] })
    let financeDataCycleB = useGetApiFinanceTotalsByKeyYearsQuery({ key: uiSelectionData.candidateId, years: [uiSelectionData.electionYear+2] })
    
    const buildFinanceData = (source: FinanceTotalsDto): IChartData[] => {
        
        const outData: Array<IChartData> = [];
        outData.push({
            label: "Indivudal Contributions",
            dataKey: ValidateToken(source.individualItemizedContributions||0),
            labelShort: "Indivudal Contributions",
            toolTipItemLabel: "Indivudal Contributions",
            toolTipItemValueLabel: "Dollars"
        })
        outData.push({
            label: "Non-Indivudal Contributions",
            dataKey: ValidateToken(source.otherPoliticalCommitteeContributions||0),
            labelShort: "Non-Indivudal Contributions",
            toolTipItemLabel: "Non-Indivudal Contributions",
            toolTipItemValueLabel: "Dollars"
        })
        return PrepareLabelsforDisplay(outData, false, 10);
    }

    //issue in rtk query where request can be shown as success, but status=pending or not fulfilled
    //
    if (financeDataCycleA.isSuccess && financeDataCycleB.isSuccess && financeDataCycleB.status == "fulfilled") {

            console.log("cycleB Data=");
            console.log(financeDataCycleB);
            let data = financeDataCycleA.data.length > 0 ? buildFinanceData(financeDataCycleA.data[0]) : buildFinanceData(financeDataCycleB.data[0]);
            return data
        }
        
        
    
        

    else {
    
        return (

            <CircularProgress />
        )
    }
}