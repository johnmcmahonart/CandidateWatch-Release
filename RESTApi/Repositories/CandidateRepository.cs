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

            List<Candidate> outList = new();
            List<CandidatebyYear> sortedCandidates = new();

            //get candidates grouped by year
            AsyncPageable<TableEntity> candidatebyYear = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq 'CandidatebyYear'");
            await foreach (var item in candidatebyYear)
            {
                sortedCandidates.Add(item.TableEntityToModel<CandidatebyYear>());
                
            }
            //get candidate records from table

            foreach (var year in years)
            {
                var i = sortedCandidates.FindIndex(x => x.Year.Equals(year));
                
                foreach (var candidateforYear in sortedCandidates[i].Candidates)
                {
                    AsyncPageable<TableEntity> candidate = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((Candidate c) => c.CandidateId)}  eq '{candidateforYear}'");
                    await foreach (var record in candidate)
                    {
                        outList.Add(record.TableEntityToModel<Candidate>());
                    }
                }
            }


            return outList.AsReadOnly();
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
