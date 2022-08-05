using Azure.Data.Tables;
namespace RESTApi.Repositories
{
    public abstract class AzureTable
    {
        private protected string? _partitionKey = default;
        private protected static TableClient _tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");

    }
}
