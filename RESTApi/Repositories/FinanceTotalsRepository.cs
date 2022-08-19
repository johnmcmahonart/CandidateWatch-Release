using Azure;
using Azure.Data.Tables;
using MDWatch.Model;
using MDWatch.Utilities;

namespace RESTApi.Repositories
{
    public class FinanceTotalsRepository : AzTable, IFinanceTotalsRepository<CandidateHistoryTotal>
    {
        private List<CandidateHistoryTotal> _inMemList = new();
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
            
            try
            {
                foreach (var item in inEntity)
                {
                    AsyncPageable<TableEntity> financeRow = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((CandidateHistoryTotal c) => c.CandidateId)}  eq '{item.CandidateId}' and {General.GetMemberName((CandidateHistoryTotal c) => c.CandidateElectionYear)}  eq '{item.CandidateElectionYear}' ");
                    await foreach (var row in financeRow)
                    {
                        await _tableClient.DeleteEntityAsync(row.PartitionKey, row.RowKey);
                        
                    }
                    

                }
            }
            catch (Exception ex)
            {
                //todo
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
            
            List<CandidateHistoryTotal> outList 
                = new List<CandidateHistoryTotal>();
            AsyncPageable<TableEntity> candidate = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((CandidateHistoryTotal c) => c.CandidateId)} eq '{key}'");
            await foreach (var historyRows in candidate)
            {
                outList.Add(historyRows.TableEntityToModel<CandidateHistoryTotal>());
            }
            return outList.AsReadOnly();
        
            
        }

        public async Task<IEnumerable<CandidateHistoryTotal>> GetbyElectionYearAsync(List<int> years)
        {

            List<CandidateHistoryTotal> outList = new();
            //is there a better way then creating an in memory copy of entire partition?
            if (!_inMemList.Any()) //check if in memory list has data, if not load from storage
            {
                AsyncPageable<TableEntity> candidates = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}'");
                await foreach (var candidate in candidates)
                {
                    _inMemList.Add(candidate.TableEntityToModel<CandidateHistoryTotal>());
                }
            }


            foreach (var year in years)
            {

                {
                    outList.AddRange(from c in _inMemList where c.ElectionYears.Contains(year) select c);
                }

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

        public async Task<IEnumerable<CandidateHistoryTotal>> GetbyCandidateandElectionYearsAsync(List<int> years, string key)
        {
            var candidateFinanceTotals = await GetbyKeyAsync(key);
            List<CandidateHistoryTotal> outList = new();
            outList.AddRange((IEnumerable<CandidateHistoryTotal>)(from c in candidateFinanceTotals where years.Contains(c.Cycle) select c));


            return outList.AsReadOnly();
        }

        public FinanceTotalsRepository()
        {
            _partitionKey = "FinanceTotals";
        }
    }
}