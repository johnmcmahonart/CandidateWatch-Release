import { Input } from '@mui/material';
import { ILineChartData } from './Interfaces/Components';

//because label names are to long from the DB we need to format them so they can be displayed
export default function PrepareLabelsforDisplay(inputData: ILineChartData[], fixLength:boolean=false): ILineChartData[] {
    const maxLength = 10;
    const outData: ILineChartData[] = [];
    //convert label names from all cap to first letter of word capitialized, make label not longer then
    //10 characters, after char 10 replace with ...
    inputData.forEach(element => {
        
        let lower = element.label.toLowerCase();
        let labelSubString = lower
        if (fixLength) {
            labelSubString = lower.substring(0, maxLength);
            if (lower.length > maxLength) {
                labelSubString += "...";
            }
        }
        
        
        const words: string[] = labelSubString.split(" ");
        let label: string = words.map((word) => (word.charAt(0).toUpperCase() + word.slice(1))).join(" ")
        outData.push({ dataKey: element.dataKey, label: label });
    });
        
        
return outData
}