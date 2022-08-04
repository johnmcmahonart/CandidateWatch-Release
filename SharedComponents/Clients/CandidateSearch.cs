using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MDWatch.Client;
using MDWatch.FECApi;
using MDWatch.Model;

namespace MDWatch.SolutionClients
{
    public class CandidateSearchClient : FECClient

    {
        public List<Candidate> Candidates => _candidates;
        private CandidateApi _apiClient;

        private string _state;
        private List<Candidate> _candidates = new List<Candidate>();

        protected override void ConfigureEndPoint()
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

        public override async Task<bool> SubmitAsync()
        {
            //get all MD candidates
            CandidatePage page = await _apiClient.CandidatesSearchGetAsync(apiKey: _apiKey, state: new List<string> { _state });

            foreach (var candidate in page.Results)
            {
                _candidates.Add(candidate);
            }

            //check if there is more then a single page of results and if so loop through all pages
            if (page.Pagination.Pages > 1)
            {
                var currentPage = page.Pagination.Page;
                while (page.Pagination.Page <= page.Pagination.Pages)
                {
                    currentPage++;
                    page = await _apiClient.CandidatesSearchGetAsync(apiKey: _apiKey, state: new List<string> { _state }, page: currentPage);
                    _candidates.AddRange(page.Results);
                }
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