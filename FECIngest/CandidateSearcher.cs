using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FECIngest.FECApi;
using FECIngest.Client;
using FECIngest.Model;
namespace FECIngest
{
    public class CandidateSearcher
   
    {

        
        private  List<Candidate> _candidates = new List<Candidate>();
        public List<Candidate> Candidates => _candidates;
        private readonly string _state;
        private readonly DateTime _startDate;
        const string apiKey = "xT2E5C0eUKvhVY74ylbGf4NWXz57XlxTkWV9pOwu";
        
        public async Task<bool> Submit()
        {
            
            Configuration candidateConfiguration = new Configuration();
            candidateConfiguration.BasePath="https://api.open.fec.gov/v1";
            //candidateConfiguration.
            candidateConfiguration.Servers.Add(new Dictionary<string, object>
            
            {
                {"url","/candidates/search" },
                {"description","candidate search endpoint" }

            }

                );
            CandidateApi candidateApi = new CandidateApi(candidateConfiguration);
            //get all MD candidates
            CandidatePage page = await candidateApi.CandidatesSearchGetAsync(apiKey:apiKey,state: new List<string>{ "MD"});
            
            foreach (var candidate in page.Results)
            {
                _candidates.Add(candidate);
            }
            return true;   
            
            
        }

        
        public CandidateSearcher(string state)
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentException($"'{nameof(state)}' cannot be null or empty.", nameof(state));
            }
            this._state = state;
            
        }
        public CandidateSearcher(string state, DateTime startDate)
        {
            if (string.IsNullOrEmpty(state))
            {
                throw new ArgumentException($"'{nameof(state)}' cannot be null or empty.", nameof(state));
            }

            this._state = state;
            this._startDate = startDate;
            
        }

    }
}
