namespace RESTApi.Repositories
{
    public interface IScheduleBOverviewRepository<T> : IRepository<T>, IGetbyKeys<T> where T : class, new()
    {
    }
}
