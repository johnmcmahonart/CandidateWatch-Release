using System;
using System.Collections.Generic;
using System.Text;
using Azure.Data.Tables;
using Azure;
using MDWatch.Utilities;
//using Azure.Identity;
namespace MDWatch
{
    public static class TablePurge
    //deletes all rows from azure table partition
    {
    public static bool Purge(string partition, string state)
        {
            TableClient tableClient = AzureUtilities.GetTableClient(state);
            Pageable<TableEntity> query = tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{partition}'");
            foreach (var r in query)
            {
                try
                {
                    tableClient.DeleteEntity(partition, r.GetString("RowKey"));
                }
                catch (Exception ex)
                {
                    
                    return false;
                }
            }
            return true;
        }
    }
}
