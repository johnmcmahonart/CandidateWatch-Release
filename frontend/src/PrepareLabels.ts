import { Input } from '@mui/material';
import { IChartData } from './Interfaces/Components';

//because label names are to long from the DB we need to format them so they can be displayed
export default function PrepareLabelsforDisplay(inputData: IChartData[], fixLength:boolean=false, maxLength:number=20): IChartData[] {
    
    const outData: IChartData[] = [];
    //convert label names from all cap to first letter of word capitialized, make label not longer then maxLength
    
    inputData.forEach(element => {
        
        let lower = element.label.toLowerCase();
        let labelSubString = lower
        const words: string[] = labelSubString.split(" ");
        
        const label: string = words.map((word) => (word.charAt(0).toUpperCase() + word.slice(1))).join(" ")
        
        if (fixLength) {
            let labelShort = label
            labelShort = label.substring(0, maxLength);
            if (label.length > maxLength) {
                labelShort += "...";
            }
            outData.push({
                dataKey: element.dataKey,
                label: label,
                labelShort: labelShort,
                toolTipItemLabel: element.toolTipItemLabel,
                toolTipItemValueLabel: element.toolTipItemValueLabel
            });
            return outData;
        }
        
        
        
        outData.push({
            dataKey: element.dataKey,
            label: label,
            labelShort: label,
            toolTipItemLabel: element.toolTipItemLabel, 
            toolTipItemValueLabel: element.toolTipItemValueLabel  });
    });
        
        
return outData
}