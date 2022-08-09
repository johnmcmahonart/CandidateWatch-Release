using Azure;
using Azure.Data.Tables;
using MDWatch.Model;
using MDWatch.Utilities;

namespace RESTApi.Repositories
{
    public class CandidateStatusRepository : AzTable, IRepository<CandidateStatus>, ICandidateStatusRepository<CandidateStatus>
    {
        public async Task<IEnumerable<CandidateStatus>> GetAllAsync()
        {
            List<CandidateStatus> outList = new List<CandidateStatus>();
            AsyncPageable<TableEntity> candidates = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}'");
            await foreach (var candidate in candidates)
            {
                outList.Add(candidate.TableEntityToModel<CandidateStatus>());
            }
            return outList.AsReadOnly();
        }

        public async Task UpdateAsync(CandidateStatus inEntity)
        {
        }

        public async Task<IEnumerable<CandidateStatus>> GetAllCandidateStatus(bool state)
        {
            //gets all candidates that have any status of 'state'
            List<CandidateStatus> outList = new List<CandidateStatus>();
            AsyncPageable<TableEntity> candidates = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' or " +
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
            TableEntity candidate = await _tableClient.GetEntityAsync<TableEntity>(_partitionKey, key);
            return (IEnumerable<CandidateStatus>)candidate.TableEntityToModel<CandidateStatus>();
        }

        public async Task AddAsync(IEnumerable<CandidateStatus> inEntity)
        {
            foreach (var item in inEntity)
            {
                TableEntity outEntity = inEntity.ModelToTableEntity(_tableClient, _partitionKey!, item.CandidateId);
                await _tableClient.AddEntityAsync(outEntity);
            }
        }

        public async Task UpdateAsync(IEnumerable<CandidateStatus> inEntity)
        {
            foreach (var item in inEntity)
            {
                TableEntity entity = await _tableClient.GetEntityAsync<TableEntity>(_partitionKey, item.CandidateId);
                entity = inEntity.ModelToTableEntity(_tableClient, _partitionKey!, item.CandidateId);

                await _tableClient.UpdateEntityAsync(entity, entity.ETag);
            }
        }

        public async Task DeleteAsync(IEnumerable<CandidateStatus> inEntity)
        {
            foreach (var item in inEntity)
            {
                TableEntity entityDelete = item.ModelToTableEntity(_tableClient, _partitionKey!, item.CandidateId);
                await _tableClient.DeleteEntityAsync(entityDelete.PartitionKey, entityDelete.RowKey);
            }
        }

        private CandidateStatusRepository()
        {
            _partitionKey = "CandidateStatus";
        }
    }
}