import { FinanceTotalsDto, useGetApiFinanceTotalsByKeyYearsQuery } from "./APIClient/MDWatch";
import { IChartData } from "./Interfaces/Components";

export function ValidateToken<T>(input: T): T {
    //if token is true pass token through, if not set as fallback
    return input;
}

//type guard
export function isChartData(test: any): test is Array<IChartData> {
    return test instanceof Array<IChartData>
}
