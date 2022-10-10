namespace RESTApi.Repositories
{
    public interface IGetbyKeysandElectionYear<T> where T : class, new()
    {
        public Task<IEnumerable<IEnumerable<T>>> GetbyKeysandElectionYearAsync(List<string> keys, int year);
    }
}