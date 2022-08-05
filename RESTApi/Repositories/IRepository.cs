using System.Linq.Expressions;

namespace RESTApi.Repositories
{
    public interface IRepository<T>
    {
        public Task<T> GetbyKeyAsync(string key);
        public Task<ICollection<T>> GetAllAsync();
        
        public Task AddAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(T entity);
    
    }
}
