using Azure;
using Azure.Data.Tables;
using MDWatch.Model;
using MDWatch.Utilities;

namespace RESTApi.Repositories
{
    public class ScheduleBDetailRepository : AzTable, IRepository<ScheduleBByRecipientID>
    {
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

        public Task<IEnumerable<ScheduleBByRecipientID>> GetAllAsync()
        {
            throw new NotImplementedException();
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

        private ScheduleBDetailRepository()
        {
            _partitionKey = "ScheduleBDetail";
        }
    }
}