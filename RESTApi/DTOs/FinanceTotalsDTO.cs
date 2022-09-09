namespace RESTApi.DTOs
{
    public record FinanceTotalsDTO
    {
        public int? ActiveThrough { get; set; }
        public string AddressState { get; set; }
        public int? CandidateElectionYear { get; set; }
        public string CandidateId { get; set; }
        public string CandidateStatus { get; set; }
        public decimal? CashOnHandEndPeriod { get; set; }
        public int Cycle { get; set; }

        public List<int> Cycles { get; set; }
        public decimal? DebtsOwedByCommittee { get; set; }
        public decimal? Disbursements { get; set; }
        public int ElectionYear { get; set; }
        public List<int> ElectionYears { get; set; }
        public bool? FederalFundsFlag { get; set; }
        public bool? HasRaisedFunds { get; set; }
        public decimal? IndividualItemizedContributions { get; set; }
        public DateTime? LastF2Date { get; set; }
        public string Name { get; set; }
        public string Office { get; set; }
        public string OfficeFull { get; set; }
        public decimal? OtherPoliticalCommitteeContributions { get; set; }
        public string Party { get; set; }
        public string PartyFull { get; set; }
        public string State { get; set; }
    }
}
