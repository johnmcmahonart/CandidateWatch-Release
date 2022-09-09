namespace RESTApi.Repositories
{
    public interface IGetbyCandidateandElectionYears<T> where T : class
        
    {
        public Task<IEnumerable<T>> GetbyCandidateandElectionYearsAsync(List<int> years, string key);
    }
}
