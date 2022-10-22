using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MDWatch.Client;
using MDWatch.FECApi;
using MDWatch.Model;

namespace MDWatch.SolutionClients
{
    //some states like CA have so many candidates, we need to write each page to retrieve to a queue so the pages can be retrived aync
    //function worker timeout can occur if we don't do this
    public class CandidateSearchClient : FECClient

    {
        public List<Candidate> Candidates => _candidates;
        private CandidateApi _apiClient;
        private readonly string apiKey;
        private string _state;
        private int _pageNumber;
        private List<Candidate> _candidates = new List<Candidate>();
        private int _totalCandidatePages;
        public int TotalPages => _totalCandidatePages;
        private protected override void ConfigureEndPoint()
        {
            _config = new Configuration();
            _config.BasePath = "https://api.open.fec.gov/v1";

            _config.Servers.Add(new Dictionary<string, object>

            {
                {"url","/candidates/search" },
                {"description","candidate search endpoint" }
            }

                );
            _apiClient = new CandidateApi(_config);
        }

        public void SetPage(int pageNumber)
        {
            _pageNumber = pageNumber;
        }
        public override async Task<bool> SubmitAsync()
        {
            _candidates = new();
            //get all MD candidates
            
            CandidatePage page = await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() =>_apiClient.CandidatesSearchGetAsync(apiKey: _apiKey, state: new List<string> { _state }, page:_pageNumber));
            _totalCandidatePages = page.Pagination.Pages;
            foreach (var candidate in page.Results)
            {
                _candidates.Add(candidate);
            }
            
            

            return true;
        }

        public CandidateSearchClient(string apiKey, string state)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            
            _state = state ?? throw new ArgumentNullException(nameof(state));
            
            ConfigureEndPoint();
        }
    }
}