namespace RESTApi.Repositories
{
    public interface ICandidateRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetbyCycle(int[] cycles);


    }
}
