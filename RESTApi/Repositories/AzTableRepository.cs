using Azure.Data.Tables;
using AutoMapper;
using MDWatch.Utilities;
using RESTApi.Mapper;
using MDWatch.Utilities;
namespace RESTApi.Repositories
{
    public abstract class AzTableRepository 
        
        
    {
        private protected string? _partitionKey = default;
        private protected static TableClient _tableClient = AzureUtilities.GetTableClient("MD");
        
    }
}
