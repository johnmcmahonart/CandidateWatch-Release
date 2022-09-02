namespace RESTApi.Repositories
{
    public interface ICandidateRepository<T>: IRepository<T>, IGetbyElectionYears<T> where T : class, new()
    {
    }
}
