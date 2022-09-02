namespace RESTApi.Repositories
{
    public interface IScheduleBDetailRepository<T>: IRepository<T>, IGetbyElectionYears<T>, IGetbyCandidateandElectionYears<T> where T: class, new()
    {
    }
}
