namespace RESTApi.Repositories
{
    public interface IGetbyCandidateandElectionYears<T> where T : class
    {
        Task<IEnumerable<T>> GetbyCandidateandElectionYearsAsync(List<int> years, string key);
    }
}
