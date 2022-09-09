using MDWatch.Model;

namespace RESTApi.DTOs
{
    public record CandidateDTO
    {
        public string CandidateId { get; set; }
        public string CandidateStatus { get; set; }
        public List<int> Cycles { get; set; }
        public string District { get; set; }
        public List<int> ElectionYears { get; set; }
        public bool HasRaisedFunds { get; set; }
        public List<int> InactiveElectionYears { get; set; }
        public DateTime? LastF2Date { get; set; }
        public DateTime? LastFileDate { get; set; }
        public string Name { get; set; }
        public string Office { get; set; }
        public string OfficeFull { get; set; }
        public string Party { get; set; }
        public string PartyFull { get; set; }
        public List<CandidatePrincipalCommittees> PrincipalCommittees { get; set; }
        public string State { get; set; }
    }
}