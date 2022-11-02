using MDWatch.Model;
using MDWatch.SolutionClients;
using Azure.Data.Tables;
using MDWatch.Utilities;
using System.Threading.Tasks;
using Azure;
using Newtonsoft;
using System.Linq.Expressions;
using System.Reflection;

namespace RESTApi.Repositories
{
    public class FinanceOverviewRepository : IFinanceOverviewRepository<CandidateFinanceOverview>
    {
        private string? _partitionKey=default;
        public async Task AddAsync(IEnumerable<CandidateFinanceOverview> inEntity)
        {
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
        }

        public async Task DeleteAsync(IEnumerable<CandidateFinanceOverview> inEntity)
        {
            foreach (var item in inEntity)
            {
                TableEntity entityDelete = item.ModelToTableEntity(IStateAzureTableClient._tableClient, _partitionKey!, item.CandidateId);
                await IStateAzureTableClient._tableClient.DeleteEntityAsync(entityDelete.PartitionKey, entityDelete.RowKey);
            }

        }

        public async Task<IEnumerable<CandidateFinanceOverview>> GetAllAsync()
        {
            List<CandidateFinanceOverview> outList = new();
            AsyncPageable<TableEntity> candidates = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}'");
            await foreach (var candidate in candidates)
            {
                outList.Add(candidate.TableEntityToModel<CandidateFinanceOverview>());
            }
            return outList.AsReadOnly();
        }

        public async Task<IEnumerable<CandidateFinanceOverview>> GetbyKeyAsync(string key)
        {
            TableEntity candidate = await IStateAzureTableClient._tableClient.GetEntityAsync<TableEntity>(_partitionKey, key);
            return  new List<CandidateFinanceOverview> { candidate.TableEntityToModel<CandidateFinanceOverview>() };
        }

        public async Task UpdateAsync(IEnumerable<CandidateFinanceOverview> inEntity)
        {
            foreach (var item in inEntity)
            {
                TableEntity entity = await IStateAzureTableClient._tableClient.GetEntityAsync<TableEntity>(_partitionKey, item.CandidateId);
                entity = inEntity.ModelToTableEntity(IStateAzureTableClient._tableClient, _partitionKey!, item.CandidateId);

                await IStateAzureTableClient._tableClient.UpdateEntityAsync(entity, entity.ETag);
            }
        }
    public FinanceOverviewRepository()
        {
            _partitionKey = "FinanceOverview";
        }
    
    }
}
