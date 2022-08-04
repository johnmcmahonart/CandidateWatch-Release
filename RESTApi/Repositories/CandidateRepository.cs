using MDWatch.Model;
using MDWatch.SolutionClients;
using Azure.Data.Tables;
using MDWatch.Utilities;
using System.Threading.Tasks;
using Azure;
using Newtonsoft;

namespace RESTApi.Repositories
{
    public class CandidateRepository : IRepository<Candidate>
    {
        private static string _partitionKey = "Candidate";
        private static TableClient _tableClient= new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
        public async Task  AddAsync(Candidate inEntity)
        {
            TableEntity outEntity = inEntity.ModelToTableEntity(_tableClient, _partitionKey, inEntity.CandidateId);
            await _tableClient.AddEntityAsync(outEntity);
        }

        void IRepository<Candidate>.Delete(Candidate entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<Candidate> GetAll()
        {
            List<Candidate> outList = new List<Candidate>();
            Pageable<TableEntity> candidates = _tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}'");
            foreach (var candidate in candidates)
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

        ICollection<Candidate> IRepository<Candidate>.GetbyQuery(IQueryable<Candidate> query)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Candidate inEntity)
        {
            TableEntity entity = await _tableClient.GetEntityAsync<TableEntity>(_partitionKey, inEntity.CandidateId);
            entity = inEntity.ModelToTableEntity(_tableClient, _partitionKey, inEntity.CandidateId);
            
            await _tableClient.UpdateEntityAsync(entity, entity.ETag);

        }
        CandidateRepository() { }
        
                    }
        
    
    
}
