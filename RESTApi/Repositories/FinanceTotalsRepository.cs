using Azure;
using Azure.Data.Tables;
using MDWatch.Model;
using MDWatch.Utilities;

namespace RESTApi.Repositories
{
    public class FinanceTotalsRepository : AzTable, IRepository<CandidateHistoryTotal>
    {
        public async Task AddAsync(IEnumerable<CandidateHistoryTotal> inEntity)
        {
            foreach (var item in inEntity)
            {
                TableEntity outEntity = item.ModelToTableEntity(_tableClient, _partitionKey!, Guid.NewGuid().ToString());
                await _tableClient.AddEntityAsync(outEntity);
            }
        }

        public async Task DeleteAsync(IEnumerable<CandidateHistoryTotal> inEntity)
        {
            //this needs to be fixed, should delete work on all rows for candidate?
            foreach (var item in inEntity)
            {
                TableEntity outEntity = inEntity.ModelToTableEntity(_tableClient, _partitionKey!, item.CandidateId);
                await _tableClient.AddEntityAsync(outEntity);
            }
        }

        public async Task<IEnumerable<CandidateHistoryTotal>> GetAllAsync()
        {
            List<CandidateHistoryTotal> outList = new List<CandidateHistoryTotal>();
            AsyncPageable<TableEntity> candidates = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}'");
            await foreach (var candidate in candidates)
            {
                outList.Add(candidate.TableEntityToModel<CandidateHistoryTotal>());
            }
            return outList.AsReadOnly();
        }

        public async Task<IEnumerable<CandidateHistoryTotal>> GetbyKeyAsync(string key)
        {
            List<CandidateHistoryTotal> outList = new List<CandidateHistoryTotal>();
            AsyncPageable<TableEntity> candidate = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((CandidateHistoryTotal c) => c.CandidateId)} eq '{key}'");
            await foreach (var historyRows in candidate)
            {
                outList.Add(historyRows.TableEntityToModel<CandidateHistoryTotal>());
            }
            return outList.AsReadOnly();
        }

        public async Task UpdateAsync(IEnumerable<CandidateHistoryTotal> inEntity)
        {
            //this probably isn't correct either. REVIEW AND FIX
            foreach (var item in inEntity)
            {
                TableEntity entity = await _tableClient.GetEntityAsync<TableEntity>(_partitionKey, item.CandidateId);
                entity = inEntity.ModelToTableEntity(_tableClient, _partitionKey!, item.CandidateId);
                await _tableClient.UpdateEntityAsync(entity, entity.ETag);
            }
        }

        public FinanceTotalsRepository()
        {
            _partitionKey = "FinanceTotals";
        }
    }
}