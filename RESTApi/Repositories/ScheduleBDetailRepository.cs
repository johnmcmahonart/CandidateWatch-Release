using Azure;
using Azure.Data.Tables;
using MDWatch.Model;
using MDWatch.Utilities;

namespace RESTApi.Repositories
{
    public class ScheduleBDetailRepository : AzTableRepository, IScheduleBDetailRepository<ScheduleBByRecipientID>
    {
        private List<ScheduleBByRecipientID> _inMemList = new();

        public async Task AddAsync(IEnumerable<ScheduleBByRecipientID> inEntity)
        {
            try
            {
                foreach (var item in inEntity)
                {
                    TableEntity outEntity = inEntity.ModelToTableEntity(_tableClient, _partitionKey!, Guid.NewGuid().ToString());
                    await _tableClient.AddEntityAsync(outEntity);
                }
            }
            catch
            {
                //todo
            }
        }

        public async Task DeleteAsync(IEnumerable<ScheduleBByRecipientID> inEntity)
        {
            foreach (var item in inEntity)
            {
                Pageable<TableEntity> scheduleBDetail = _tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.RecipientId)}  eq '{item.RecipientId}'");
                foreach (var disbursement in scheduleBDetail)
                {
                    await _tableClient.DeleteEntityAsync(disbursement.PartitionKey, disbursement.RowKey);
                }
            }
        }

        public async Task<IEnumerable<ScheduleBByRecipientID>> GetAllAsync()
        {
            List<ScheduleBByRecipientID> outList = new List<ScheduleBByRecipientID>();
            AsyncPageable<TableEntity> candidates = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}'");
            await foreach (var candidate in candidates)
            {
                outList.Add(candidate.TableEntityToModel<ScheduleBByRecipientID>());
            }
            return outList.AsReadOnly();
        }

        public async Task<IEnumerable<ScheduleBByRecipientID>> GetbyKeyAsync(string key)
        {
            List<ScheduleBByRecipientID> outList = new();
            TableEntity candidateScheduleBOverview = await _tableClient.GetEntityAsync<TableEntity>("ScheduleBOverview", key);
            AsyncPageable<TableEntity> scheduleBDetail = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.RecipientId)}  eq '{candidateScheduleBOverview.TableEntityToModel<ScheduleBCandidateOverview>().PrincipalCommitteeId}'");
            await foreach (var disbursement in scheduleBDetail)
            {
                outList.Add(disbursement.TableEntityToModel<ScheduleBByRecipientID>());
            }
            return outList.AsReadOnly();
        }

        public async Task<IEnumerable<ScheduleBByRecipientID>> GetbyElectionYearsAsync(List<int> years)
        {
            List<ScheduleBByRecipientID> outList = new();
            List<CandidatebyYear> sortedCandidates = new();

            //get candidates grouped by year
            AsyncPageable<TableEntity> candidatebyYear = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq 'CandidatebyYear'");
            await foreach (var item in candidatebyYear)
            {
                sortedCandidates.Add(item.TableEntityToModel<CandidatebyYear>());
                
            }
            //get candidate ScheduleB records from table

            foreach (var year in years)
            {
                var i = sortedCandidates.FindIndex(x => x.Year.Equals(year));

                foreach (var candidateforYear in sortedCandidates[i].Candidates)
                {
                    //get recipient ID that matches candidateID
                    AsyncPageable<TableEntity> candidate = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq 'ScheduleBOverview' and {General.GetMemberName((ScheduleBCandidateOverview c) => c.CandidateId)}  eq '{candidateforYear}'");
                    await foreach (var record in candidate)
                    {
                        string committeeId = record.TableEntityToModel<ScheduleBCandidateOverview>().PrincipalCommitteeId;
                        
                        //get ScheduleBDetail data

                        AsyncPageable<TableEntity> scheduleBDetails = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.RecipientId)}  eq '{committeeId}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.Cycle)} eq {year} ");
                        await foreach (var row in scheduleBDetails)
                        {
                            outList.Add(row.TableEntityToModel<ScheduleBByRecipientID>());
                        }
                    }
                }
            }

            return outList.AsReadOnly();
        }

        public async Task<IEnumerable<ScheduleBByRecipientID>> GetbyCandidateandElectionYearsAsync(List<int> years, string key)
        {
            IEnumerable<ScheduleBByRecipientID> recipient = await GetbyKeyAsync(key);
            
            List<ScheduleBByRecipientID> outList = new();
            foreach (var year in years)
            {
                outList.AddRange((IEnumerable<ScheduleBByRecipientID>)(from c in recipient where c.Cycle.Equals(years) select c));
            }
            

            return outList.AsReadOnly();
        }

        public async Task UpdateAsync(IEnumerable<ScheduleBByRecipientID> inEntity)
        {
            foreach (var recipient in inEntity)
            {
                AsyncPageable<TableEntity> scheduleBDetailRow = _tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.RecipientId)}  eq '{recipient.RecipientId}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.CommitteeId)}  eq '{recipient.CommitteeId}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.Cycle)}  eq '{recipient.Cycle}' ");
                await foreach (var disbursement in scheduleBDetailRow)
                {
                    TableEntity entity = await _tableClient.GetEntityAsync<TableEntity>(_partitionKey, disbursement.RowKey);

                    entity = recipient.ModelToTableEntity(_tableClient, _partitionKey!, disbursement.RowKey);
                    await _tableClient.UpdateEntityAsync(entity, entity.ETag);
                }
            }
        }

        public ScheduleBDetailRepository()
        {
            _partitionKey = "ScheduleBDetail";
        }
    }
}