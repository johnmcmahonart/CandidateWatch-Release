import React from 'react';
//import { IChartToolTip } from '../Interfaces/Components'
export default function ChartToolTip ({ active, payload}:any) {
    if (active && payload && payload.length) {
        
        return (
            <div style={{background:"white"} } >

                
                <p>{payload[0].payload.toolTipItemLabel}:{payload[0].payload.label}</p>
                <p>{payload[0].payload.toolTipItemValueLabel}:{payload[0].value}</p>
            </div>

        );
    }

    return null;
};