import { FinanceTotalsDto, useGetApiFinanceTotalsByKeyYearsQuery } from "../APIClient/MDWatch";

export function useElectionCycle(candidateId: string, year: number): FinanceTotalsDto | boolean {
    //checks if the current election cycle is correct for senators or if we need to use the alternate 6 year cycle
    //senators alternate election cycles, and there isn't a way to know within data contained in FEC API which cycle a senator was elected in,
    //if no data is returned for a given cycle, we assume they were elected in the other 6 year cycle (current even numbered year +2)
    let financeDataCycleA = useGetApiFinanceTotalsByKeyYearsQuery({ key: candidateId, years: [year] })
    let financeDataCycleB = useGetApiFinanceTotalsByKeyYearsQuery({ key: candidateId, years: [year + 2] })

    if (financeDataCycleA.isSuccess && financeDataCycleB.isSuccess && financeDataCycleB.status == "fulfilled") {
        const data:FinanceTotalsDto = financeDataCycleA.data.length > 0 ? financeDataCycleA.data[0] : financeDataCycleB.data[0]
        return data
    }
    return false;
}