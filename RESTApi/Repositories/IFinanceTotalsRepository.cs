using MDWatch.Model;
namespace RESTApi.Repositories

{
    public interface IFinanceTotalsRepository<T>: IRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetbyElectionYearAsync(List<int> years);

    }
}
