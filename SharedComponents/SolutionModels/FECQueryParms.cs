using System;
using System.Collections.Generic;
using System.Text;

namespace FECIngest.Model
{
    public record class FECQueryParms
    //data model for FEC Query parameters, this is used to wrap optional parameters in an object since FEC methods contain many optional parms
    {
        public string CandidateId { get; set; }
        public string State { get; set; }
        public string CommitteeId { get; set; }
        public int PageIndex { get; set; }
    }

}

