namespace RESTApi.Repositories
{
    public interface IGetbyElectionYears<T> where T : class, new()

    {
        public Task<IEnumerable<T>> GetbyElectionYearsAsync(List<int> years);
    }
}