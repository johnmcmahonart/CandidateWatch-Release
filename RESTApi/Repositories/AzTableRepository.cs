using Azure.Data.Tables;
using AutoMapper;
using MDWatch.Utilities;
using RESTApi.Mapper;
namespace RESTApi.Repositories
{
    public abstract class AzTableRepository 
        
        
    {
        private protected string? _partitionKey = default;
        private protected static TableClient _tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
        
    }
}
