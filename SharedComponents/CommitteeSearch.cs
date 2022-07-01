using FECIngest.Client;
using FECIngest.FECApi;
using FECIngest.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FECIngest
{
    public class CommitteeSearch : FECClient, IFECQueryParms
    {
        public List<Committee> Committees => _committees;

        private Dictionary<string, string> _queryParms;
        
        private List<Committee> _committees = new List<Committee>();
        
        private CommitteeApi _apiClient;

        public void SetQuery(Dictionary<string, string> parms)
        {
            _queryParms = parms ?? throw new ArgumentNullException(nameof(parms));
        }
        public override async Task<bool> Submit()
        {
            if (_queryParms == null)
            {
                throw new ArgumentException("Query parameters must be set. Use SetQuery before submission");
                
            }
            else
            {
                CommitteePage page = await _apiClient.CommitteesGetAsync(apiKey: _apiKey, candidateId: new List<String> { _queryParms["candidateId"] });

                if (page.Results.Count > 0)
                {
                    foreach (var committee in page.Results)
                    {
                        _committees.Add(committee);
                    }
                    return true;
                }
                
                else
                    return false;
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

        public CommitteeSearch(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            ConfigureEndPoint();
        }
    }
}