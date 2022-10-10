namespace RESTApi.Repositories
{
    public interface IScheduleBDetailRepository<T>: IRepository<T>, IGetbyElectionYears<T>, IGetbyCandidateandElectionYears<T>, IGetbyKeys<T>, IGetbyKeysandElectionYear<T> where T: class, new()
    {
    }
}
