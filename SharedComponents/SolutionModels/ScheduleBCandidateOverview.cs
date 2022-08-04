using System;
using System.Collections.Generic;
using System.Text;

namespace MDWatch.Model
{
    public record class ScheduleBCandidateOverview
    {
        public int TotalDisbursements { get; set; }
        public int TotalResultPages { get; set; }
        public string CandidateId { get; set; }
        public string PrincipalCommitteeId { get; set; }
    }
}
