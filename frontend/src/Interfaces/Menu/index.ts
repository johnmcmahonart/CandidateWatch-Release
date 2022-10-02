import {CallerTypes } from '../../Enums'
export interface INavElement {
    text: string;
    candidateId: string | undefined | null;

}
export interface IUIData {
    electionYear: number;
    candidateId?: string;
    
}
