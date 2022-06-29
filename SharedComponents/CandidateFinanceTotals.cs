using FECIngest.Client;
using FECIngest.FECApi;
using FECIngest.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace FECIngest
{
    public class CandidateFinanceTotals : IFECSearch
    {
        public Configuration Config => _configuration;
        public List<CandidateHistoryTotal> Contributions => _contributions;
        private Configuration _configuration;
        private List<CandidateHistoryTotal> _contributions = new List<CandidateHistoryTotal>();
        public string APIKey => _APIKey;
        private string _APIKey;
        private string _candidateId;

        public Decimal GetTotalNonIndividualContributions()
        {
            if (String.IsNullOrEmpty(_candidateId))
            {
                throw new Exception("CandidateId must be set before calling this method");
            }
            else
            {
                var individualContributions = from c in _contributions where c.CandidateId.Contains(_candidateId) select c.OtherPoliticalCommitteeContributions;
                Decimal total = 0;
                foreach (Decimal i in individualContributions)
                {
                    total += i;
                }
                return total;
            };
        }
        public Decimal GetTotalIndividualContributions()
        {
            if (String.IsNullOrEmpty(_candidateId))
            {
                throw new Exception("CandidateId must be set before calling this method");
            }
            else
            {
                var individualContributions = from c in _contributions where c.CandidateId.Contains(_candidateId) select c.IndividualItemizedContributions;
                Decimal total = 0;
                foreach (Decimal i in individualContributions)
                {
                    total += i;
                }
                return total;
            };
        }

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
            CandidateHistoryTotalPage page = await candidate.CandidatesTotalsGetAsync(apiKey: _APIKey, candidateId: new List<string> { _candidateId });

            foreach (var i in page.Results)
            {
                _contributions.Add(i);
            }

            return true;
        }
        public CandidateFinanceTotals(string APIKey)
        {
            _APIKey = APIKey ?? throw new ArgumentNullException(nameof(APIKey));
        }
    }
}