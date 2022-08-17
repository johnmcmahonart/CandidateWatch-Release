namespace RESTApi.Repositories
{
    public interface IGetbyElectionYears<T>: IRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetbyElectionYearAsync(List<int> years);


    }
}
