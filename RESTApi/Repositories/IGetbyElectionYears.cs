using Azure.Data.Tables;
using MDWatch.Model;

namespace RESTApi.Repositories
{
    public interface IGetbyElectionYears<T>where T : class
    {
        Task<IEnumerable<T>> GetbyElectionYearsAsync(List<int> years);
        
    }
}
