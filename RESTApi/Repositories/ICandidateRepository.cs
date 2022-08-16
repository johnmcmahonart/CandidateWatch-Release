namespace RESTApi.Repositories
{
    public interface ICandidateRepository<T>: IRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetbyElectionYearAsync(List<int> years);


    }
}
