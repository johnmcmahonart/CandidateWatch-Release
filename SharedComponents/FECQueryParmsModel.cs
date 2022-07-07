﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FECIngest
{
    public class FECQueryParmsModel
    //data model for FEC Query parameters, this is used to wrap optional parameters in an object since FEC methods contain many optional parms
    {
        public string CandidateId { get; set; }
        public string State { get; set; }
        public string CommitteeId { get; set; }
    }

}
