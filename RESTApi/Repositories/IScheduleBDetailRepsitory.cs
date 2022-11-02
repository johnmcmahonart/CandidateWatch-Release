namespace RESTApi.Repositories
{
    public interface IScheduleBDetailRepository<T>: IRepository<T>, IGetbyElectionYears<T>, IGetbyCandidateandElectionYears<T>, IGetbyKeys<T>, IGetbyKeysandElectionYear<T>, IStateAzureTableClient where T: class, new()
    {
    }
}
