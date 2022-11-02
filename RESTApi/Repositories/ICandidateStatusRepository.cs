namespace RESTApi.Repositories
{
    public interface ICandidateStatusRepository<T>: IRepository<T>,IStateAzureTableClient where T : class
    {
        public Task<IEnumerable<T>> GetAllCandidateStatus(bool state);


    }
}
