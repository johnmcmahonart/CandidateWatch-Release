namespace RESTApi.Repositories
{
    public interface ICandidateStatusRepository<T>: IRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetAllCandidateStatus(bool state);


    }
}
