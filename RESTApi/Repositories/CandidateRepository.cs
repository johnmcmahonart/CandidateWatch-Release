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
    public class CandidateRepository :AzTable, ICandidateRepository<Candidate>
    {

        public async Task  AddAsync(IEnumerable<Candidate> inEntity)
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

        public async Task DeleteAsync(IEnumerable<Candidate> entity)
        {
            foreach (var item in entity)
            {
                TableEntity entityDelete = entity.ModelToTableEntity(_tableClient, _partitionKey!, item.CandidateId);
                await _tableClient.DeleteEntityAsync(entityDelete.PartitionKey, entityDelete.RowKey);
            }
            
        }

        public async Task<IEnumerable<Candidate>> GetAllAsync()
        {
            List<Candidate> outList = new List<Candidate>();
            AsyncPageable<TableEntity> candidates = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}'");
            await foreach (var candidate in candidates)
            {
                outList.Add(candidate.TableEntityToModel<Candidate>());
            }
            return outList.AsReadOnly();
        }

        public async Task<Candidate> GetbyKeyAsync(string key)
        {
            try
            {
                TableEntity candidate = await _tableClient.GetEntityAsync<TableEntity>(_partitionKey, key);
                return candidate.TableEntityToModel<Candidate>();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        public async Task<IEnumerable<Candidate>> GetbyCycleAsync(int[] cycles)
        {
            List<Candidate> outList = new List<Candidate>();
            foreach (var cycle in cycles)
            {
                
                AsyncPageable<TableEntity> candidates = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and Cycles-json substringof '{cycle.ToString()}'");
                await foreach (var candidate in candidates)
                {
                    outList.Add(candidate.TableEntityToModel<Candidate>());
                }
                
            }
            return outList.AsReadOnly();
        }

        public async Task UpdateAsync(IEnumerable<Candidate> inEntity)
        {
            foreach (var item in inEntity)
            {
                TableEntity entity = await _tableClient.GetEntityAsync<TableEntity>(_partitionKey, item.CandidateId);
                entity = inEntity.ModelToTableEntity(_tableClient, _partitionKey!, item.CandidateId);

                await _tableClient.UpdateEntityAsync(entity, entity.ETag);
            }
            

        }
        public CandidateRepository() 
        {
            _partitionKey= "Candidate";
        }
        
                    }
        
    
    
}
