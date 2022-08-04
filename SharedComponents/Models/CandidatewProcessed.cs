namespace MDWatch.Model
{
    //adds validation parms for downstream worker processes
    public partial class Candidate
    {
        public bool CommitteeProcessed { get; set; }
        public bool FinanceTotalProcessed { get; set; }

        public bool ScheduleBProcessed { get; set; }
    }
}