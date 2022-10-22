using System;
using System.Collections.Generic;
using System.Text;

namespace MDWatch.Model
{
    public record class CandidateQueueMessage
    //used to build partition of candidates grouped by year, needed for efficient grouping of candidates by election year 
    {
        public string CandidateId { get; set; }
        public string State { get; set; }

    }
}
