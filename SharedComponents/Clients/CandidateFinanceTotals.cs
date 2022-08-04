using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDWatch.Client;
using MDWatch.FECApi;
using MDWatch.Model;

namespace MDWatch.SolutionClients
{
    public class CandidateFinanceTotalsClient : FECClient, IFECQueryParms
    {
        public List<CandidateHistoryTotal> Contributions => _contributions;

        private List<CandidateHistoryTotal> _contributions = new List<CandidateHistoryTotal>();

        private FECQueryParms _queryParms;

        private CandidateApi _apiClient;

        public Decimal? GetTotalNonIndividualContributions()
        {
            if (_queryParms == null)
            {
                throw new Exception("queryparms must be set before calling this method");
            }
            else
            {
                var nonIndividualContributions = from c in _contributions where c.CandidateId.Contains(_queryParms.CandidateId) select c.OtherPoliticalCommitteeContributions;
                return nonIndividualContributions.Sum();
            };
        }

        public Decimal? GetTotalIndividualContributions()
        {
            if (_queryParms == null)
            {
                throw new Exception("queryparms must be set before calling this method");
            }
            else
            {
                var individualContributions = from c in _contributions where c.CandidateId.Contains(_queryParms.CandidateId) select c.IndividualItemizedContributions;
                return individualContributions.Sum();
            };
        }

        public void SetQuery(FECQueryParms parms)
        {
            _queryParms = parms ?? throw new ArgumentException(nameof(parms));
        }

        
        private protected override void ConfigureEndPoint()
        {
            _config = new Configuration();
            _config.BasePath = "https://api.open.fec.gov/v1";
            _config.Servers.Add(new Dictionary<string, object>
            {
                {"url","/candidates/totals/" },
                {"description","candidate totals" }
            }
            );
            _apiClient = new CandidateApi(_config);
        }

        public override async Task SubmitAsync()
        {
            if (_queryParms == null)
            {
                throw new ArgumentException("Query parameters must be set. Use SetQuery before submission");
            }
            else
            {
                CandidateHistoryTotalPage page = await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() => _apiClient.CandidatesTotalsGetAsync(apiKey: _apiKey, candidateId: new List<string> { _queryParms.CandidateId }));
                if (page.Results.Count > 0)
                {
                    _contributions.AddRange(page.Results);
                }
            }
        }

        public CandidateFinanceTotalsClient(string APIKey)
        {
            _apiKey = APIKey ?? throw new ArgumentNullException(nameof(APIKey));
            ConfigureEndPoint();
        }
    }
}