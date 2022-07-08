using FECIngest.Client;
using FECIngest.FECApi;
using FECIngest.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace FECIngest
{
    public class ScheduleBDisbursement : FECClient, IFECQueryParms, IFECPagination
    {
        private DisbursementsApi _apiClient;
        private FECQueryParmsModel _queryParms;
        private List<ScheduleBByRecipientID> _disbursements = new List<ScheduleBByRecipientID>();
        private ScheduleBByRecipientIDPage _page;
        private int _currentPage = 1;
        public int TotalDisbursementsforCandidate
        {
            get
            {
                if (_totalDisbursementsforCandidate == 0)
                {
                    throw new Exception("Please submit query to API to retrieve result count");


                }
                else
                {
                    return _totalDisbursementsforCandidate;
                }

                }
            }
        private int _totalDisbursementsforCandidate;
        public List<ScheduleBByRecipientID> Disbursements => _disbursements;
        public void SetQuery(FECQueryParmsModel parms)
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
                _page = await _apiClient.SchedulesScheduleBByRecipientIdGetAsync(apiKey: _apiKey, recipientId: new List<String> { _queryParms.CommitteeId }, page: _currentPage);
                _totalDisbursementsforCandidate = _page.Pagination.Count;
                if (_page.Results.Count > 0)
                {
                    _disbursements.AddRange(_page.Results);

                    return true;
                }
                
                else
                {
                    return false;
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

        public async Task<IFECResultPage> GetNextPage()
        {
            _currentPage++;
            
            await SharedComponents.PollyPolicy.GetDefault.ExecuteAsync(() => this.Submit());

            var scheduleBResult = new FECPageResultScheduleB(_page);
            
            if (scheduleBResult.IsLastPage)
            {
                _currentPage = 1;
            }
            
            return scheduleBResult;
            
        }
        public ScheduleBDisbursement(string APIKey)
        {
            _apiKey = APIKey ?? throw new ArgumentNullException(nameof(APIKey));

            ConfigureEndPoint();
        }
    }
}