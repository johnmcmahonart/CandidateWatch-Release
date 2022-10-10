namespace RESTApi.Repositories
{
    public interface IGetbyKeys<T> where T : class, new()
    {
        public Task<IEnumerable<IEnumerable<T>>> GetbyKeysAsync(List<string> keys);
    }
}
