using MDWatch.Model;
using MDWatch.SolutionClients;
using Azure.Data.Tables;
using MDWatch.Utilities;
using System.Threading.Tasks;
using Azure;
using Newtonsoft;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using System.Reflection;

namespace RESTApi.Repositories
{
    public class ScheduleBOverview : AzTable, IRepository<ScheduleBCandidateOverview>
    {
        public async Task AddAsync(IEnumerable<ScheduleBCandidateOverview> inEntity)
        {
            try
            {
                foreach (var item in inEntity)
                {
                    TableEntity outEntity = inEntity.ModelToTableEntity(_tableClient, _partitionKey!, item.CandidateId);
                    await _tableClient.AddEntityAsync(outEntity);
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
                TableEntity entityDelete = item.ModelToTableEntity(_tableClient, _partitionKey!, item.CandidateId);
                await _tableClient.DeleteEntityAsync(entityDelete.PartitionKey, entityDelete.RowKey);
            }
            
        }

        public async Task<IEnumerable<ScheduleBCandidateOverview>> GetAllAsync()
        {
            List<ScheduleBCandidateOverview> outList = new ();
            AsyncPageable<TableEntity> candidates = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}'");
            await foreach (var candidate in candidates)
            {
                outList.Add(candidate.TableEntityToModel<ScheduleBCandidateOverview>());
            }
            return outList.AsReadOnly();
        }

        public async Task<IEnumerable<ScheduleBCandidateOverview>> GetbyKeyAsync(string key)
        {
            TableEntity candidate = await _tableClient.GetEntityAsync<TableEntity>(_partitionKey, key);
            return (IEnumerable<ScheduleBCandidateOverview>)candidate.TableEntityToModel<ScheduleBCandidateOverview>();
        }

        public async Task UpdateAsync(IEnumerable<ScheduleBCandidateOverview> inEntity)
        {
            foreach (var item in inEntity)
            {
                TableEntity entity = await _tableClient.GetEntityAsync<TableEntity>(_partitionKey, item.CandidateId);
                entity = inEntity.ModelToTableEntity(_tableClient, _partitionKey!, item.CandidateId);

                await _tableClient.UpdateEntityAsync(entity, entity.ETag);
            }
            
        }
        ScheduleBOverview()
        {
            _partitionKey = "ScheduleBOverview";
        }
    }
}
