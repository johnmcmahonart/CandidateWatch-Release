using Azure;
using Azure.Data.Tables;
using MDWatch.Model;
using MDWatch.Utilities;

namespace RESTApi.Repositories
{
    public class ScheduleBDetailRepository : IScheduleBDetailRepository<ScheduleBByRecipientID>
    {
        private List<ScheduleBByRecipientID> _inMemList = new();
        private string? _partitionKey = default;
        public async Task AddAsync(IEnumerable<ScheduleBByRecipientID> inEntity)
        {
            try
            {
                foreach (var item in inEntity)
                {
                    TableEntity outEntity = inEntity.ModelToTableEntity(IStateAzureTableClient._tableClient, _partitionKey!, Guid.NewGuid().ToString());
                    await IStateAzureTableClient._tableClient.AddEntityAsync(outEntity);
                }
            }
            catch
            {
                //TODO
            }
        }

        public async Task DeleteAsync(IEnumerable<ScheduleBByRecipientID> inEntity)
        {
            foreach (var item in inEntity)
            {
                Pageable<TableEntity> scheduleBDetail = IStateAzureTableClient._tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.RecipientId)}  eq '{item.RecipientId}'");
                foreach (var disbursement in scheduleBDetail)
                {
                    await IStateAzureTableClient._tableClient.DeleteEntityAsync(disbursement.PartitionKey, disbursement.RowKey);
                }
            }
        }

        public async Task<IEnumerable<ScheduleBByRecipientID>> GetAllAsync()
        {
            List<ScheduleBByRecipientID> outList = new List<ScheduleBByRecipientID>();
            AsyncPageable<TableEntity> candidates = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}'");
            await foreach (var candidate in candidates)
            {
                outList.Add(candidate.TableEntityToModel<ScheduleBByRecipientID>());
            }
            return outList.AsReadOnly();
        }

        public async Task<IEnumerable<IEnumerable<ScheduleBByRecipientID>>> GetbyKeysAsync(List<string> keys)
        {
            List<List<ScheduleBByRecipientID>> outList = new();
            foreach (var candidate in keys)
            {
                TableEntity candidateScheduleBOverview = await IStateAzureTableClient._tableClient.GetEntityAsync<TableEntity>("ScheduleBOverview", candidate);
                AsyncPageable<TableEntity> scheduleBDetail = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.RecipientId)}  eq '{candidateScheduleBOverview.TableEntityToModel<ScheduleBCandidateOverview>().PrincipalCommitteeId}'");
                List<ScheduleBByRecipientID> candidateScheduleB = new();
                await foreach (var disbursement in scheduleBDetail)
                {
                    candidateScheduleB.Add(disbursement.TableEntityToModel<ScheduleBByRecipientID>());
                }
                outList.Add(candidateScheduleB);
            }

            return outList.AsReadOnly();
        }

        public async Task<IEnumerable<IEnumerable<ScheduleBByRecipientID>>> GetbyKeysandElectionYearAsync(List<string> keys, int year)
        {
            
            List<List<ScheduleBByRecipientID>> outList = new();
            foreach (var candidate in keys)
            {
                IEnumerable<ScheduleBByRecipientID> recipient = await GetbyKeyAsync(candidate);

                List<ScheduleBByRecipientID> candidateScheduleB = new();

                candidateScheduleB.AddRange((IEnumerable<ScheduleBByRecipientID>)(from c in recipient where c.Cycle.Equals(year) select c));
                outList.Add(candidateScheduleB);
            }

            return outList.AsReadOnly();
        }

        public async Task<IEnumerable<ScheduleBByRecipientID>> GetbyKeyAsync(string key)
        {
            List<ScheduleBByRecipientID> outList = new();
            AsyncPageable<TableEntity> candidateScheduleBOverview = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq 'ScheduleBOverview' and RowKey eq '{key}'");

            var candidateScheduleBOverviewResult = await candidateScheduleBOverview.FirstOrDefaultAsync<TableEntity>();
            if (candidateScheduleBOverviewResult != null)
            {
                //TableEntity candidateScheduleBOverview = await _tableClient.GetEntityAsync<TableEntity>("ScheduleBOverview", key);
                AsyncPageable<TableEntity> scheduleBDetail = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.RecipientId)}  eq '{candidateScheduleBOverviewResult.TableEntityToModel<ScheduleBCandidateOverview>().PrincipalCommitteeId}'");
                await foreach (var disbursement in scheduleBDetail)
                {
                    outList.Add(disbursement.TableEntityToModel<ScheduleBByRecipientID>());
                }
                return outList.AsReadOnly();
            }
            outList.Add(new ScheduleBByRecipientID());
            return outList.AsReadOnly();
        }

        public async Task<IEnumerable<ScheduleBByRecipientID>> GetbyElectionYearsAsync(List<int> years)
        {
            List<ScheduleBByRecipientID> outList = new();
            List<CandidatebyYear> sortedCandidates = new();

            //get candidates grouped by year
            AsyncPageable<TableEntity> candidatebyYear = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq 'CandidatebyYear'");
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
                    AsyncPageable<TableEntity> candidate = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq 'ScheduleBOverview' and {General.GetMemberName((ScheduleBCandidateOverview c) => c.CandidateId)}  eq '{candidateforYear}'");
                    await foreach (var record in candidate)
                    {
                        string committeeId = record.TableEntityToModel<ScheduleBCandidateOverview>().PrincipalCommitteeId;

                        //get ScheduleBDetail data

                        AsyncPageable<TableEntity> scheduleBDetails = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.RecipientId)}  eq '{committeeId}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.Cycle)} eq {year} ");
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
                outList.AddRange((IEnumerable<ScheduleBByRecipientID>)(from c in recipient where c.Cycle.Equals(year) select c));
            }

            return outList.AsReadOnly();
        }

        public async Task UpdateAsync(IEnumerable<ScheduleBByRecipientID> inEntity)
        {
            foreach (var recipient in inEntity)
            {
                AsyncPageable<TableEntity> scheduleBDetailRow = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_partitionKey}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.RecipientId)}  eq '{recipient.RecipientId}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.CommitteeId)}  eq '{recipient.CommitteeId}' and {General.GetMemberName((ScheduleBByRecipientID c) => c.Cycle)}  eq '{recipient.Cycle}' ");
                await foreach (var disbursement in scheduleBDetailRow)
                {
                    TableEntity entity = await IStateAzureTableClient._tableClient.GetEntityAsync<TableEntity>(_partitionKey, disbursement.RowKey);

                    entity = recipient.ModelToTableEntity(IStateAzureTableClient._tableClient, _partitionKey!, disbursement.RowKey);
                    await IStateAzureTableClient._tableClient.UpdateEntityAsync(entity, entity.ETag);
                }
            }
        }

        public ScheduleBDetailRepository()
        {
            _partitionKey = "ScheduleBDetail";
        }
    }
}