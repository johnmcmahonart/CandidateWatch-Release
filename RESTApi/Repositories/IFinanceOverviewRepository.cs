namespace RESTApi.Repositories
{
    public interface IFinanceOverviewRepository<T>: IRepository<T>, IStateAzureTableClient where T : class, new ()
    {
    }
}
