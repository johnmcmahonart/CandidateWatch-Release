using System;
using System.Collections.Generic;
using System.Text;
using Azure.Data.Tables;

namespace MDWatch.Model
{
    public record class GetElectionYearConfig
    {
        public List<int> Years { get; set; }        
        public string Key { get; set; }
        public TableClient Client { get; set; } 
        public string Partition { get; set; }
    }
}
