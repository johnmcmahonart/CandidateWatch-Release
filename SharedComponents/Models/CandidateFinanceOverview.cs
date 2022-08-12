using System;
using System.Collections.Generic;
using System.Text;

namespace MDWatch.Model
{
    public record class CandidateFinanceOverview
    {
        public string CandidateId { get; set; }
        public decimal TotalNonIndividualContributions { get; set; }
        public decimal TotalIndividualContributions { get; set; }
    }
}
