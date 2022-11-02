using Azure;
using Azure.Data.Tables;
using MDWatch.Model;
using MDWatch.Utilities;

namespace RESTApi.Repositories
{
    public class CandidateStatusRepository : IRepository<CandidateStatus>, ICandidateStatusRepository<CandidateStatus>
    {
        private string? _partitionKey = default;
        public async Task<IEnumerable<CandidateStatus>> GetAllAsync()
        {
            List<CandidateStatus> outList = new List<CandidateStatus>();
            AsyncPageable<TableEntity> candidates = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}'");
            await foreach (var candidate in candidates)
            {
                outList.Add(candidate.TableEntityToModel<CandidateStatus>());
            }
            return outList.AsReadOnly();
        }

        

        public async Task<IEnumerable<CandidateStatus>> GetAllCandidateStatus(bool state)
        {
            //gets all candidates that have any status of 'state'
            List<CandidateStatus> outList = new List<CandidateStatus>();
            AsyncPageable<TableEntity> candidates = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' or " +
                $"{General.GetMemberName((CandidateStatus c) => c.ScheduleBProcessed)} eq {state} or " +
                $"{General.GetMemberName((CandidateStatus c) => c.CommitteeProcessed)} eq {state} or " +
                $"{General.GetMemberName((CandidateStatus c) => c.FinanceTotalProcessed)} eq {state}");
            await foreach (var candidate in candidates)
            {
                outList.Add(candidate.TableEntityToModel<CandidateStatus>());
            }
            return outList.AsReadOnly();
        }

        public async Task<IEnumerable<CandidateStatus>> GetbyKeyAsync(string key)
        {
            TableEntity candidate = await IStateAzureTableClient._tableClient.GetEntityAsync<TableEntity>(_partitionKey, key);
            return new List<CandidateStatus> { candidate.TableEntityToModel<CandidateStatus>() };
        }

        public async Task AddAsync(IEnumerable<CandidateStatus> inEntity)
        {
            foreach (var item in inEntity)
            {
                TableEntity outEntity = inEntity.ModelToTableEntity(IStateAzureTableClient._tableClient, _partitionKey!, item.CandidateId);
                await IStateAzureTableClient._tableClient.AddEntityAsync(outEntity);
            }
        }

        public async Task UpdateAsync(IEnumerable<CandidateStatus> inEntity)
        {
            foreach (var item in inEntity)
            {
                TableEntity entity = await IStateAzureTableClient._tableClient.GetEntityAsync<TableEntity>(_partitionKey, item.CandidateId);
                entity = inEntity.ModelToTableEntity(IStateAzureTableClient._tableClient, _partitionKey!, item.CandidateId);

                await IStateAzureTableClient._tableClient.UpdateEntityAsync(entity, entity.ETag);
            }
        }

        public async Task DeleteAsync(IEnumerable<CandidateStatus> inEntity)
        {
            foreach (var item in inEntity)
            {
                TableEntity entityDelete = item.ModelToTableEntity(IStateAzureTableClient._tableClient, _partitionKey!, item.CandidateId);
                await IStateAzureTableClient._tableClient.DeleteEntityAsync(entityDelete.PartitionKey, entityDelete.RowKey);
            }
        }

        public CandidateStatusRepository()
        {
            _partitionKey = "CandidateStatus";
        }
    }
}