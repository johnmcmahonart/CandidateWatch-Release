namespace RESTApi.Repositories
{
    public interface IFinanceTotalsRepository<T>: IRepository<T>, IGetbyElectionYears<T>, IGetbyCandidateandElectionYears<T> where T : class
    {
    }
}
