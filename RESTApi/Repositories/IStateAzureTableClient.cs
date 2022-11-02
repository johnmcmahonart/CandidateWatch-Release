using Azure.Data.Tables;
using AutoMapper;
using MDWatch.Utilities;
using RESTApi.Mapper;

namespace RESTApi.Repositories
{
    public interface IStateAzureTableClient 
        
        
    {
        
        private protected static TableClient _tableClient =AzureUtilities.GetTableClient("MD");
        public void SetState(string state)
        {
            _tableClient = AzureUtilities.GetTableClient(state);
        }
    }
}
