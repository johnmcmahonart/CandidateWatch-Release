using MDWatch.Client;
using MDWatch.FECApi;
using MDWatch.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace MDWatch
{
    public class CandidateFinanceTotalsSearch : IFECSearch
    {
        public Configuration Config => _configuration;
        public List<CandidateHistoryTotal> Contributions => _contributions;
        private Configuration _configuration;
        private List<CandidateHistoryTotal> _contributions = new List<CandidateHistoryTotal>();
        public string APIKey => _APIKey;
        private string _APIKey;
        private string _candidateId;

        public void SetCandidate(string candidateID)
        {
            _candidateId = candidateID ?? throw new ArgumentException(nameof(candidateID));
        }
        public async Task<bool> Submit()
        {
            _configuration = new Configuration();
            _configuration.BasePath = "https://api.open.fec.gov/v1";
            _configuration.Servers.Add(new Dictionary<string, object>
            {
                {"url","/candidates/totals/" },
                {"description","candidate totals" }
            }
            );
            CandidateApi candidate = new CandidateApi(_configuration);
            CandidateHistoryTotalPage page =await  candidate.CandidatesTotalsGetAsync(apiKey: _APIKey, candidateId:new List<string> {_candidateId });
            if (_candidateId =="S6MD03441")
            {
                Console.WriteLine("found candidate");
            }
                foreach (var i in page.Results)
            {
                _contributions.Add(i);
            }
            
            
            return true;
        }
        public CandidateFinanceTotalsSearch(string APIKey)
        {
            _APIKey = APIKey ?? throw new ArgumentNullException(nameof(APIKey));
        }
    }
}