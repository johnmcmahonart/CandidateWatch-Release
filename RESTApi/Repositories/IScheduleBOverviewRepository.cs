namespace RESTApi.Repositories
{
    public interface IScheduleBOverviewRepository<T> : IRepository<T>, IGetbyKeys<T>, IStateAzureTableClient where T : class, new()
    {
    }
}
