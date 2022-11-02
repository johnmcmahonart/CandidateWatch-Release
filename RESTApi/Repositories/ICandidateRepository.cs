namespace RESTApi.Repositories
{
    public interface ICandidateRepository<T>: IRepository<T>, IGetbyElectionYears<T>, IStateAzureTableClient where T : class, new()
    {
    }
}
