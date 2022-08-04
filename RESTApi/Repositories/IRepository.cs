namespace RESTApi.Repositories
{
    public interface IRepository<T>
    {
        public Task<T> GetbyKeyAsync(string key);
        public ICollection<T> GetAll();
        public ICollection<T> GetbyQuery (IQueryable<T> query);
        public Task AddAsync(T entity);
        public Task UpdateAsync(T entity);
        public void Delete(T entity);
    
    }
}
