namespace RESTApi.Repositories
{
    public interface IFinanceTotalsRepository<T>: IRepository<T>, IGetbyElectionYears<T>, IGetbyCandidateandElectionYears<T>,IGetbyKeys<T>,IGetbyKeysandElectionYear<T> where T : class, new()
    {
    }
}
