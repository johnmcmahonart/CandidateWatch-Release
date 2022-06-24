using System;
using System.Collections.Generic;
using System.Text;
using Azure.Data.Tables;
using Azure;
using Azure.Identity;
namespace FECIngest
{
    public static class TablePurge
    //deletes all rows from azure table partition
    {
    public static bool Purge(string partition)
        {
            TableClient tableClient = new TableClient("UseDevelopmentStorage=true", "MDWatchDEV");
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
