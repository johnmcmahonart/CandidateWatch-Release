using MDWatch.Model;

namespace RESTApi.Repositories
{
    public interface IGetbyElectionYears<T>where T : class
    {
        Task<IEnumerable<T>> GetbyElectionYearAsync(List<int> years);
        
    }
}
