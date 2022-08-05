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
    public class CandidateRepository :AzureTable, IRepository<Candidate>
    {

        public async Task  AddAsync(Candidate inEntity)
        {
            TableEntity outEntity = inEntity.ModelToTableEntity(_tableClient, _partitionKey!, inEntity.CandidateId);
            await _tableClient.AddEntityAsync(outEntity);
        }

        public async Task DeleteAsync(Candidate entity)
        {
            TableEntity entityDelete = entity.ModelToTableEntity(_tableClient, _partitionKey!, entity.CandidateId);
            await _tableClient.DeleteEntityAsync(entityDelete.PartitionKey, entityDelete.RowKey);
        }

        public async Task<ICollection<Candidate>> GetAllAsync()
        {
            List<Candidate> outList = new List<Candidate>();
            AsyncPageable<TableEntity> candidates = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}'");
            await foreach (var candidate in candidates)
            {
                outList.Add(candidate.TableEntityToModel<Candidate>());
            }
            return outList;
        }

        public async Task<Candidate> GetbyKeyAsync(string key)
        {
            TableEntity candidate = await _tableClient.GetEntityAsync<TableEntity>(_partitionKey,key);
            return candidate.TableEntityToModel<Candidate>();
        
        }

        public async Task< ICollection<Candidate>> GetCandidatesProcessedAsync(PropertyInfo property, bool state)
        {
            List<Candidate> outList = new List<Candidate>();
            AsyncPageable<TableEntity> candidates = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {property.Name} eq {state}");
            await foreach (var candidate in candidates)
            {
                outList.Add(candidate.TableEntityToModel<Candidate>());
            }
            return outList;
            
        }

        public async Task UpdateAsync(Candidate inEntity)
        {
            TableEntity entity = await _tableClient.GetEntityAsync<TableEntity>(_partitionKey, inEntity.CandidateId);
            entity = inEntity.ModelToTableEntity(_tableClient, _partitionKey!, inEntity.CandidateId);
            
            await _tableClient.UpdateEntityAsync(entity, entity.ETag);

        }
        CandidateRepository() 
        {
            _partitionKey= "Candidate";
        }
        
                    }
        
    
    
}
