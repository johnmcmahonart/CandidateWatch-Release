namespace RESTApi.DTOs
{
    public record ScheduleBDetailDTO
    {
        public string CommitteeId { get; set; }
        public string CommitteeName { get; set; }
        public int Cycle { get; set; }
        public string RecipientId { get; set; }
        public string RecipientName { get; set; }

    }
}
