using Azure;
using Azure.Data.Tables;
using MDWatch.Model;
using MDWatch.Utilities;

namespace RESTApi.Repositories
{
    public class FinanceTotalsRepository : IFinanceTotalsRepository<CandidateHistoryTotal>
    {
        private string? _partitionKey = default;
                                    

        public async Task AddAsync(IEnumerable<CandidateHistoryTotal> inEntity)
        {
            foreach (var item in inEntity)
            {
                TableEntity outEntity = item.ModelToTableEntity(IStateAzureTableClient._tableClient, _partitionKey!, Guid.NewGuid().ToString());
                await IStateAzureTableClient._tableClient.AddEntityAsync(outEntity);
            }
        }

        public async Task DeleteAsync(IEnumerable<CandidateHistoryTotal> inEntity)
        {
            try
            {
                foreach (var item in inEntity)
                {
                    AsyncPageable<TableEntity> financeRow = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((CandidateHistoryTotal c) => c.CandidateId)}  eq '{item.CandidateId}' and {General.GetMemberName((CandidateHistoryTotal c) => c.CandidateElectionYear)}  eq '{item.CandidateElectionYear}' ");
                    await foreach (var row in financeRow)
                    {
                        await IStateAzureTableClient._tableClient.DeleteEntityAsync(row.PartitionKey, row.RowKey);
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
            AsyncPageable<TableEntity> candidates = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}'");
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
            AsyncPageable<TableEntity> candidate = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((CandidateHistoryTotal c) => c.CandidateId)} eq '{key}'");
            await foreach (var historyRows in candidate)
            {
                outList.Add(historyRows.TableEntityToModel<CandidateHistoryTotal>());
            }
            return outList.AsReadOnly();
        }

        public async Task<IEnumerable<CandidateHistoryTotal>> GetbyElectionYearsAsync(List<int> years)
        {
            List<CandidateHistoryTotal> outList = new();
            List<CandidatebyYear> sortedCandidates = new();

            //get candidates grouped by year
            AsyncPageable<TableEntity> candidatebyYear = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq 'CandidatebyYear'");
            await foreach (var item in candidatebyYear)
            {
                sortedCandidates.Add(item.TableEntityToModel<CandidatebyYear>());
            }
            //get candidate finace records from table

            foreach (var year in years)
            {
                var i = sortedCandidates.FindIndex(x => x.Year.Equals(year));
                foreach (var candidateforYear in sortedCandidates[i].Candidates)
                {
                    AsyncPageable<TableEntity> candidate = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((CandidateHistoryTotal c) => c.CandidateId)}  eq '{candidateforYear}'");
                    await foreach (var record in candidate)
                    {
                        outList.Add(record.TableEntityToModel<CandidateHistoryTotal>());
                    }
                }
            }

            return outList.AsReadOnly();
        }

        public async Task UpdateAsync(IEnumerable<CandidateHistoryTotal> inEntity)
        {
            //this probably isn't correct either. REVIEW AND FIX
            foreach (var item in inEntity)
            {
                TableEntity entity = await IStateAzureTableClient._tableClient.GetEntityAsync<TableEntity>(_partitionKey, item.CandidateId);
                entity = inEntity.ModelToTableEntity(IStateAzureTableClient._tableClient, _partitionKey!, item.CandidateId);
                await IStateAzureTableClient._tableClient.UpdateEntityAsync(entity, entity.ETag);
            }
        }

        public async Task<IEnumerable<CandidateHistoryTotal>> GetbyCandidateandElectionYearsAsync(List<int> years, string key)
        {
            IEnumerable<CandidateHistoryTotal> candidate = await GetbyKeyAsync(key);
            List<CandidateHistoryTotal> outList = new();
            foreach (var year in years)
            {
                outList.AddRange((IEnumerable<CandidateHistoryTotal>)(from c in candidate where c.Cycle.Equals(year) select c));
            }

            return outList.AsReadOnly();
        }

        public async Task<IEnumerable<IEnumerable<CandidateHistoryTotal>>> GetbyKeysAsync(List<string> keys)
        {
            {
                List<List<CandidateHistoryTotal>> outList = new();
                List<CandidateHistoryTotal> candidateList = new();
                foreach (var candidate in keys)
                {
                    //use query instead of GetEntity since the entity may not exist
                    AsyncPageable<TableEntity> candidateEntity = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and CandidateId eq '{candidate}'");

                    var candidateResult = await candidateEntity.FirstOrDefaultAsync<TableEntity>();
                    if (candidateResult != null)
                    {
                        candidateList.Add(candidateResult.TableEntityToModel<CandidateHistoryTotal>());
                    }
                }
                outList.Add(candidateList);
                return outList;
            }
        }

        public async Task<IEnumerable<IEnumerable<CandidateHistoryTotal>>> GetbyKeysandElectionYearAsync(List<string> keys, int year)
        {
            List<List<CandidateHistoryTotal>> outList = new();

            {
                IEnumerable<IEnumerable<CandidateHistoryTotal>> candidates = await GetbyKeysAsync(keys);

                List<CandidateHistoryTotal> candidateData = new();
                foreach (var candidate in candidates)
                {
                    candidateData.AddRange((IEnumerable<CandidateHistoryTotal>)(from c in candidate where c.Cycle.Equals(year) select c));
                    outList.Add(candidateData);
                }
            }

            return outList.AsReadOnly();
        }

        public FinanceTotalsRepository()
        {
            _partitionKey = "FinanceTotals";
        }
    }
}