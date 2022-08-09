namespace RESTApi.Repositories
{
    public interface ICandidateStatusRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetAllCandidateStatus(bool state);


    }
}
