using Azure;
using Azure.Data.Tables;
using MDWatch.Model;
using MDWatch.Utilities;

namespace RESTApi.Repositories
{
    public class ScheduleBOverviewRepository : IScheduleBOverviewRepository<ScheduleBCandidateOverview>
    {
        private string? _partitionKey = default;
        public async Task AddAsync(IEnumerable<ScheduleBCandidateOverview> inEntity)
        {
            try
            {
                foreach (var item in inEntity)
                {
                    TableEntity outEntity = inEntity.ModelToTableEntity(IStateAzureTableClient._tableClient, _partitionKey!, item.CandidateId);
                    await IStateAzureTableClient._tableClient.AddEntityAsync(outEntity);
                }
            }
            catch
            {
                //todo
            }
        }

        public async Task DeleteAsync(IEnumerable<ScheduleBCandidateOverview> inEntity)
        {
            foreach (var item in inEntity)
            {
                TableEntity entityDelete = item.ModelToTableEntity(IStateAzureTableClient._tableClient, _partitionKey!, item.CandidateId);
                await IStateAzureTableClient._tableClient.DeleteEntityAsync(entityDelete.PartitionKey, entityDelete.RowKey);
            }
        }

        public async Task<IEnumerable<ScheduleBCandidateOverview>> GetAllAsync()
        {
            List<ScheduleBCandidateOverview> outList = new();
            AsyncPageable<TableEntity> candidates = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}'");
            await foreach (var candidate in candidates)
            {
                outList.Add(candidate.TableEntityToModel<ScheduleBCandidateOverview>());
            }
            return outList.AsReadOnly();
        }

        public async Task<IEnumerable<ScheduleBCandidateOverview>> GetbyKeyAsync(string key)
        {
            TableEntity candidate = await IStateAzureTableClient._tableClient.GetEntityAsync<TableEntity>(_partitionKey, key);

            return new List<ScheduleBCandidateOverview> { candidate.TableEntityToModel<ScheduleBCandidateOverview>() };
        }

        public async Task<IEnumerable<IEnumerable<ScheduleBCandidateOverview>>> GetbyKeysAsync(List<string> keys)
        //because not all candidates have scheduleB data, we need to handle what happens when the entity doesn't exist in the table
        //there is not agreement on how this should be handlded by the tablestorage api from MS, so the approach taken may not be ideal
        //https://github.com/Azure/azure-sdk-for-net/issues/16251
        {
            List<List<ScheduleBCandidateOverview>> outList = new();
            List<ScheduleBCandidateOverview> candidateList = new();
            foreach (var candidate in keys)
            {
                //use query instead of GetEntity since the entity may not exist
                AsyncPageable<TableEntity> candidateEntity = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and RowKey eq '{candidate}'");

                var candidateResult = await candidateEntity.FirstOrDefaultAsync<TableEntity>();
                if (candidateResult != null)
                {
                    candidateList.Add(candidateResult.TableEntityToModel<ScheduleBCandidateOverview>());
                }
            }
            outList.Add(candidateList);
            return outList;
        }

        public async Task UpdateAsync(IEnumerable<ScheduleBCandidateOverview> inEntity)
        {
            foreach (var item in inEntity)
            {
                TableEntity entity = await IStateAzureTableClient._tableClient.GetEntityAsync<TableEntity>(_partitionKey, item.CandidateId);
                entity = inEntity.ModelToTableEntity(IStateAzureTableClient._tableClient, _partitionKey!, item.CandidateId);

                await IStateAzureTableClient._tableClient.UpdateEntityAsync(entity, entity.ETag);
            }
        }

        public ScheduleBOverviewRepository()
        {
            _partitionKey = "ScheduleBOverview";
        }
    }
}