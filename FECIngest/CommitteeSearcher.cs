using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FECIngest.Client;
using FECIngest.FECApi;
using FECIngest.Model;


namespace FECIngest
{
    public class CommitteeSearcher
    {
        public string APIKey => _apiKey;
        private string _candidateID;
        private string _apiKey;
        private List<Committee> _committees = new List<Committee>();
        public List<Committee> Committees => _committees;



        public void SetCandidate(string candidateID)
        {
            if (string.IsNullOrEmpty(candidateID))
            {
                throw new ArgumentException($"'{nameof(candidateID)}' cannot be null or empty.", nameof(candidateID));
            }
            else
            {
                _candidateID = candidateID;
            }
        }
        public async Task<bool> Submit()
        {
            if (string.IsNullOrEmpty(_candidateID))
            {
                throw new ArgumentException($"'{nameof(_candidateID)}' cannot be null or empty. Use SetCandidate to set which candidate we are interested in", nameof(_candidateID));
            }
            Configuration committeeConfiguration = new Configuration();
            committeeConfiguration.BasePath = "https://api.open.fec.gov/v1";
            
            committeeConfiguration.Servers.Add(new Dictionary<string, object>

            {
                {"url","/committees" },
                {"description","committees search endpoint" }
            }

                );
           

            CommitteeApi committeeApi = new CommitteeApi(committeeConfiguration);
            //get all committees for a given candidate
            CommitteePage page = await committeeApi.CommitteesGetAsync(apiKey: _apiKey,candidateId:new List<String> { _candidateID });

            if (page.Results.Count >0)
            {
                foreach (var committee in page.Results)
                {
                    _committees.Add(committee);
                }
            }
            


            return true;
        }
        public CommitteeSearcher(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException($"'{nameof(apiKey)}' cannot be null or empty.", nameof(apiKey));
            }
            else
            {
                _apiKey = apiKey;
            }


        }
    }
}
