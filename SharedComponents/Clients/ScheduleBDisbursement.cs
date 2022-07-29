using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FECIngest.Client;
using FECIngest.FECApi;
using FECIngest.Model;

namespace FECIngest.SolutionClients
{
    public class ScheduleBDisbursementClient : FECClient, IFECQueryParms
    {
        private DisbursementsApi _apiClient;
        private FECQueryParms _queryParms;
        private List<ScheduleBByRecipientID> _disbursements;
        private ScheduleBByRecipientIDPage _page;
        private int _totalDisbursementsforCandidate;
        private int _totalPages;
        public int TotalPages => _totalPages;
        public int TotalDisbursementsforCandidate => _totalDisbursementsforCandidate;

        public List<ScheduleBByRecipientID> Disbursements => _disbursements;

        public void SetQuery(FECQueryParms parms)
        {
            _queryParms = parms ?? throw new ArgumentNullException(nameof(parms));
        }

        public override async Task SubmitAsync()
        {
            _disbursements = new List<ScheduleBByRecipientID>();

            if (_queryParms == null)
            {
                throw new ArgumentException("Query parameters must be set. Use SetQuery before submission");
            }
            else
            {
                _page = await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() => _apiClient.SchedulesScheduleBByRecipientIdGetAsync(apiKey: _apiKey, recipientId: new List<String> { _queryParms.CommitteeId }, page: _queryParms.PageIndex));
                _totalDisbursementsforCandidate = _page.Pagination.Count;
                _totalPages = _page.Pagination.Pages;
                if (_page.Results.Count > 0)
                {
                    _disbursements.AddRange(_page.Results);
                }
            }
        }

        protected override void ConfigureEndPoint()
        {
            _config = new Configuration();
            _config.BasePath = "https://api.open.fec.gov/v1";
            _config.Servers.Add(new Dictionary<string, object>
            {
                {"url","/schedules/schedule_b/by_recipient_id" },
                {"description","ScheduleB Disbursements" }
            }
            );
            _apiClient = new DisbursementsApi(_config);
        }

        public ScheduleBDisbursementClient(string APIKey)
        {
            _apiKey = APIKey ?? throw new ArgumentNullException(nameof(APIKey));

            ConfigureEndPoint();
        }
    }
}