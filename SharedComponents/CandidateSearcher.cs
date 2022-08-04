using MDWatch.Client;
using MDWatch.FECApi;
using MDWatch.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDWatch
{
    public class CandidateSearch : IFECSearch

    {
        
        public List<Candidate> Candidates => _candidates;
        public Configuration Configuration => _candidateConfiguration;
        public string APIKey => _apiKey;
        private string _apiKey;
        private string _state;
        private DateTime _startDate;
        private List<Candidate> _candidates = new List<Candidate>();
        private Configuration _candidateConfiguration;
        public async Task<bool> Submit()
        {
            _candidateConfiguration = new Configuration();
            _candidateConfiguration.BasePath = "https://api.open.fec.gov/v1";
            //candidateConfiguration.
            _candidateConfiguration.Servers.Add(new Dictionary<string, object>

            {
                {"url","/candidates/search" },
                {"description","candidate search endpoint" }
            }

                );
            CandidateApi candidateApi = new CandidateApi(_candidateConfiguration);
            //get all MD candidates
            CandidatePage page = await candidateApi.CandidatesSearchGetAsync(apiKey: _apiKey, state: new List<string> { _state });

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
                    page = await candidateApi.CandidatesSearchGetAsync(apiKey: _apiKey, state: new List<string> { _state }, page: currentPage);
                    foreach (var candidate in page.Results)
                    {
                        _candidates.Add(candidate);
                    }
                }
            }

            return true;
        }

        public CandidateSearcher(string apiKey, string state)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException($"'{nameof(apiKey)}' cannot be null or empty.", nameof(apiKey));
            }
            else
            {
                _apiKey = apiKey;
            }

            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentException($"'{nameof(state)}' cannot be null or empty.", nameof(state));
            }
            else
            {
                _state = state;
            }
        }

        
    }
}