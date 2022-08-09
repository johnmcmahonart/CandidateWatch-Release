using Azure.Data.Tables;
namespace RESTApi.Repositories
{
    public abstract class AzTable
    {
        private protected string? _partitionKey = default;
        private protected static TableClient _tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
        
    }
}
