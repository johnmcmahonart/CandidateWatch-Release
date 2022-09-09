namespace RESTApi.DTOs
{
    public record CandidateUIDTO
    {
    public string CandidateId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }    
        public string Party { get; set; }
        public Boolean WasElected { get; set; }
        public string District { get; set; }
    }
}