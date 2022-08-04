using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MDWatch.Client;
using MDWatch.FECApi;
using MDWatch.Model;

namespace MDWatch.SolutionClients
{
    public class CommitteeSearchClient : FECClient, IFECQueryParms
    {
        public List<Committee> Committees => _committees;

        private FECQueryParms _queryParms;

        private List<Committee> _committees = new List<Committee>();

        private CommitteeApi _apiClient;

        public void SetQuery(FECQueryParms parms)
        {
            _queryParms = parms ?? throw new ArgumentNullException(nameof(parms));
        }

        public override async Task SubmitAsync()
        {
            if (_queryParms == null)
            {
                throw new ArgumentException("Query parameters must be set. Use SetQuery before submission");
            }
            else
            {
                CommitteePage page = await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() => _apiClient.CommitteesGetAsync(apiKey: _apiKey, candidateId: new List<String> { _queryParms.CandidateId }));

                if (page.Results.Count > 0)
                {
                    _committees.AddRange(page.Results);
                }
            }
        }

        protected override void ConfigureEndPoint()
        {
            _config = new Configuration();
            _config.BasePath = "https://api.open.fec.gov/v1";

            _config.Servers.Add(new Dictionary<string, object>

            {
                {"url","/committees" },
                {"description","committees search endpoint" }
            }

                );
            _apiClient = new CommitteeApi(_config);
        }

        public CommitteeSearchClient(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            ConfigureEndPoint();
        }
    }
}