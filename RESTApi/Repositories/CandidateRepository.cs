using MDWatch.Model;
using MDWatch.SolutionClients;
using Azure.Data.Tables;
using MDWatch.Utilities;
using System.Threading.Tasks;
using Azure;
using Newtonsoft;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using AutoMapper;
using System.Diagnostics;
namespace RESTApi.Repositories
{
    public class CandidateRepository :AzTableRepository,ICandidateRepository<Candidate>
    {

        List<Candidate> _inMemList = new List<Candidate>();
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

        public async Task <IEnumerable<Candidate>> GetbyKeyAsync(string key)
        {
            TableEntity candidate = await _tableClient.GetEntityAsync<TableEntity>(_partitionKey, key);
            return new List<Candidate> { candidate.TableEntityToModel<Candidate>() };
            
        }

        public async Task<IEnumerable<Candidate>> GetbyElectionYearsAsync(List<int> years)
        {
            var watch = Stopwatch.StartNew();
            List<Candidate> outList = new();
            if (!_inMemList.Any())
            {
                _inMemList.AddRange(await LoadPartitiontoMemory.Read<Candidate>(_tableClient, _partitionKey));
            }

            foreach (var year in years)
            {
                
                //repository helper may not be needed, this was to used to help reduce the time for the linq query to run but long
                //query time may have been caused by an a different function
                var candidatesbyYear = RepositoryHelper.SortCandidatesByYear(_inMemList);
                foreach (var item in candidatesbyYear.year[year] )
                {
                    outList.Add((from c in _inMemList where c.CandidateId.Equals(item) select c).First());
                }
            }
            watch.Stop();

            return outList;
            

        }

        public Task UpdateAsync(IEnumerable<Candidate> inEntity)
        {
            throw new NotImplementedException();
        }

        public CandidateRepository()
        {
            _partitionKey= "Candidate";
        }
        
                    }
        
    
    
}
