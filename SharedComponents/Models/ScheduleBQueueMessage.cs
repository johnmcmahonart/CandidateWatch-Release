using System;

namespace SharedComponents.Models
{
    public record class ScheduleBQueueMessage
    {
        public string CommitteeId { get; set; }
        public string State { get; set; }
        public Int32 PageIndex { get; set; }
        
    }
}