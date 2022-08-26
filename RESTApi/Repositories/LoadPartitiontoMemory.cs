using Azure;
using Azure.Data.Tables;

namespace RESTApi.Repositories
{
    public static class LoadPartitiontoMemory
    {
        //some partitions are large enough it makes sense to load them into memory temporarily instead of many queries over the wire
        public static async Task<IEnumerable<T>> Read<T>(TableClient tableClient, string partition) where T : class, new()
        {
            List<T> inMemList = new();
            T objInstance = new();
            dynamic dynObj = objInstance;

            AsyncPageable<TableEntity> candidates = tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{partition}'");
            await foreach (var candidate in candidates)
            {
                inMemList.Add((T)MDWatch.Utilities.AzTableUtilitites.TableEntityToModel(candidate, dynObj));
                
            }

            return inMemList;
        }
    }
}