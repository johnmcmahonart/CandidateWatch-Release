using System;

namespace SharedComponents.Models
{
    public record class StateUpdateLog
    {
        public DateTime UpdateTime { get; set; }
        public String State { get; set; }   
        
        public int FinanceTotalsCount { get; set; }
        public int ScheduleBCount { get; set; }
        public int CommitteesCount { get; set; }
    }
}