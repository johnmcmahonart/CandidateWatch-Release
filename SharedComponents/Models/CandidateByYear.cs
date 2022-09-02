using System;
using System.Collections.Generic;
using System.Text;

namespace MDWatch.Model
{
    public record class CandidatebyYear
    //used to build partition of candidates grouped by year, needed for efficient grouping of candidates by election year 
    {
        public Dictionary<int, List<string>> year { get; set; }
    
    }
}
