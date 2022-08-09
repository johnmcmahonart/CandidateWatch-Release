using System.Linq.Expressions;

namespace RESTApi.Repositories
{
    public interface IRepository<T>
    {
        public Task<IEnumerable<T>> GetbyKeyAsync(string key);
        public Task<IEnumerable<T>> GetAllAsync();
        
        public Task AddAsync(IEnumerable<T> entity);
        public Task UpdateAsync(IEnumerable<T> entity);
        public Task DeleteAsync(IEnumerable<T> entity);
    
    }
}
