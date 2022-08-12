using System.Linq.Expressions;

namespace RESTApi.Repositories
{
    public interface IRepository<T>
    {
        public Task<T> GetbyKeyAsync(string key);
        public Task<IEnumerable<T>> GetAllAsync();
        
        public Task AddAsync(IEnumerable<T> inEntity);
        public Task UpdateAsync(IEnumerable<T> inEntity);
        public Task DeleteAsync(IEnumerable<T> inEntity);
    
    }
}
