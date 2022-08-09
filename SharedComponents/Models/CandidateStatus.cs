namespace MDWatch.Model
{
    //adds validation parms for downstream worker processes
    public class CandidateStatus
    {
        public string CandidateId { get; set; }
        public bool CommitteeProcessed { get; set; }
        public bool FinanceTotalProcessed { get; set; }

        public bool ScheduleBProcessed { get; set; }
    }
}