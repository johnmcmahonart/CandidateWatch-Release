using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Azure.Data.Tables;
using Azure.Identity;
using Azure.Storage.Queues;
using MDWatch.Model;
using Newtonsoft.Json;
using SharedComponents.Models;

namespace MDWatch.Utilities
{
    public static class AzureUtilities
    {
        public static string MakeCandidateQueueMessage(string candidateId, string state)
        {
            return candidateId + "," + state;
        }
        public static CandidateQueueMessage ParseCandidateQueueMessage(string queueMessage)
        {
            var splitMessage = queueMessage.Split(',');
            CandidateQueueMessage outMessage = new CandidateQueueMessage
            {
                CandidateId = splitMessage[0],
                State = splitMessage[1]
            };
            return outMessage;
        }

        public static string MakeStateCandidatesQueueMessage(string state, int page)
        {
            return state + "," + page.ToString();
        }
        public static StateCandidatesQueueMessage ParseStateCandidatesQueueMessage(string queueMessage)
        {
            var splitMessage = queueMessage.Split(',');
            int page = 0;
            Int32.TryParse(splitMessage[1], out page);
            StateCandidatesQueueMessage outMessage = new StateCandidatesQueueMessage
                        {
                State = splitMessage[0],
                Page = page
            };
            return outMessage;
        }
        public static QueueClient GetQueueClient(string queueName)
        {
            if (String.Equals(General.GetBuildEnv(), "Debug"))
            {
                QueueClient queueClient = new QueueClient("UseDevelopmentStorage=true", queueName);
                return queueClient;
            }
            else
            {
                string queueBase = "https://stcandidatewatchdata01.queue.core.windows.net/";
                QueueClient queueClient = new QueueClient(new Uri(queueBase + queueName), new DefaultAzureCredential());

                return queueClient;
            }
        }
        public static TableClient GetTableClient(string state)
        {
            if (String.Equals(General.GetBuildEnv(), "Debug"))
            {
                TableClient tableClient = new TableClient("UseDevelopmentStorage=true", state + General.EnvVars["dev_table_affix"].ToString());
                return tableClient;
                // TableClient tableClient = new TableClient(new Uri("https://stcandidatewatchdata01.table.core.windows.net/" + state + General.GetConfigurationValue("production_table_affix")),
                //state + General.GetConfigurationValue("production_table_affix"), new DefaultAzureCredential());
                //return tableClient;
            }
            else
            {
                TableClient tableClient = new TableClient(new Uri("https://stcandidatewatchdata01.table.core.windows.net/" + state + General.EnvVars["production_table_affix"].ToString()),
                    state + General.EnvVars["production_table_affix"].ToString(), new DefaultAzureCredential());
                return tableClient;
            }
        }
        public static object AddUTC(this object inputObj)
        {
            var obj = inputObj;
            Type objType = obj.GetType();
            List<PropertyInfo> props = new List<PropertyInfo>(objType.GetProperties());

            //get only properties that are of Datetime

            foreach (var prop in props)
            {
                if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(DateTime) & prop.GetValue(obj) != null)
                {
                    DateTime time = (DateTime)prop.GetValue(obj);
                    //create new DateTime with UTC time and save result back to obj
                    DateTime newTime = DateTime.SpecifyKind(time, DateTimeKind.Utc);
                    prop.SetValue(obj, newTime);
                }
            }
            return obj;
        }
        private static TableEntity BuildEntity(string partitionKey, string rowKey, object source)
        {
            //map object property types to supported TableEntity types, Serialize to json where needed and populate row/columns for writing to table
            //if value of property is null then skip

            Type objType = source.GetType();
            List<PropertyInfo> props = new List<PropertyInfo>(objType.GetProperties());
            TableEntity tableEntity = new TableEntity(partitionKey, rowKey);

            foreach (var prop in props)
            {
                if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(DateTime) | prop.PropertyType == typeof(DateTime) & prop.GetValue(source) != null)
                {
                    tableEntity.Add(prop.Name, prop.GetValue(source));
                }
                else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(bool) | prop.PropertyType == typeof(bool) & prop.GetValue(source) != null)
                {
                    tableEntity.Add(prop.Name, prop.GetValue(source));
                }
                else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(double) | prop.PropertyType == typeof(double) & prop.GetValue(source) != null)
                {
                    tableEntity.Add(prop.Name, prop.GetValue(source));
                }
                else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(Guid) | prop.PropertyType == typeof(Guid) & prop.GetValue(source) != null)
                {
                    tableEntity.Add(prop.Name, prop.GetValue(source));
                }
                else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(int) | prop.PropertyType == typeof(int) & prop.GetValue(source) != null)
                {
                    tableEntity.Add(prop.Name, prop.GetValue(source));
                }
                else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(Int32) | prop.PropertyType == typeof(Int32) & prop.GetValue(source) != null)
                {
                    tableEntity.Add(prop.Name, prop.GetValue(source));
                }
                else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(Int64) | prop.PropertyType == typeof(Int64) & prop.GetValue(source) != null)
                {
                    tableEntity.Add(prop.Name, prop.GetValue(source));
                }
                else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(long) | prop.PropertyType == typeof(long) & prop.GetValue(source) != null)
                {
                    tableEntity.Add(prop.Name, prop.GetValue(source));
                }
                else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(Decimal) & prop.GetValue(source) != null)
                {
                    tableEntity.Add(prop.Name, Decimal.ToDouble((decimal)prop.GetValue(source)));
                }
                else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(Double) & prop.GetValue(source) != null)
                {
                    tableEntity.Add(prop.Name, prop.GetValue(source));
                }
                else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(string) | prop.PropertyType == typeof(string) & prop.GetValue(source) != null)
                {
                    tableEntity.Add(prop.Name, prop.GetValue(source));
                }
                else if (prop.GetValue(source) != null) //is complex type, so we Serialize the object to json and store as string
                {
                    //need to change default serialization settings to prevent self reference looping
                    //this may no longer be necessary as this was added because of a bug in a previous version
                    string jsonString = JsonConvert.SerializeObject(prop.GetValue(source), Formatting.None, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    tableEntity.Add(prop.Name + "Json", jsonString); //add -json to column name so when we read the data back out we know to deserialize the string to json
                }
            }
            return tableEntity;
        }

        public static T TableEntityToModel<T>(this TableEntity tableEntity, T inObj) where T : new()
        {
            dynamic outObj = new T();
            Type objType = inObj.GetType();
            List<PropertyInfo> properties = new List<PropertyInfo>(objType.GetProperties());
            foreach (var property in properties)
            {
                if (property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)) && property.PropertyType != typeof(string))
                //check if property is IENumerable and not a string, if so we need to deserialize from table storage to rehydrate
                {
                    var json = new Object();

                    var hasValue = tableEntity.TryGetValue(property.Name + "Json", out json);
                    if (hasValue)
                    {
                        var deserialized = JsonConvert.DeserializeObject((string)json, property.PropertyType);

                        outObj.GetType().GetProperty(property.Name).SetValue(outObj, deserialized);
                    }
                }
                else if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(Decimal) | property.PropertyType == typeof(decimal)) //convert decimal to double since table storage only supports double
                {
                    object propertyValue = new();

                    var hasValue = tableEntity.TryGetValue(property.Name, out propertyValue);
                    if (hasValue)
                    {
                        outObj.GetType().GetProperty(property.Name).SetValue(outObj, Convert.ToDecimal(propertyValue), null);
                    }
                }
                else if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime) | property == typeof(DateTime)) //DateTime offset to DateTime conversion
                {
                    object propertyValue = new();

                    var hasValue = tableEntity.TryGetValue(property.Name, out propertyValue);
                    if (hasValue)
                    {
                        DateTimeOffset timeOffset = (DateTimeOffset)propertyValue;
                        outObj.GetType().GetProperty(property.Name).SetValue(outObj, timeOffset.UtcDateTime, null);
                    }
                }
                else
                {
                    object propertyValue = new();

                    var hasValue = tableEntity.TryGetValue(property.Name, out propertyValue);
                    if (hasValue)
                    {
                        outObj.GetType().GetProperty(property.Name).SetValue(outObj, propertyValue, null);
                    }
                }
            }

            return outObj;
        }

        public static T TableEntityToModel<T>(this TableEntity tableEntity) where T : new()
        {
            T outObj = new();
            Type objType = typeof(T);
            List<PropertyInfo> properties = new List<PropertyInfo>(objType.GetProperties());
            foreach (var property in properties)
            {
                if (property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)) && property.PropertyType != typeof(string))
                //check if property is IENumerable and not a string, if so we need to deserialize from table storage to rehydrate
                {
                    var json = new Object();

                    var hasValue = tableEntity.TryGetValue(property.Name + "Json", out json);
                    if (hasValue)
                    {
                        var deserialized = JsonConvert.DeserializeObject((string)json, property.PropertyType);

                        outObj.GetType().GetProperty(property.Name).SetValue(outObj, deserialized);
                    }
                }
                else if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(Decimal) | property.PropertyType == typeof(decimal)) //convert decimal to double since table storage only supports double
                {
                    object propertyValue = new();

                    var hasValue = tableEntity.TryGetValue(property.Name, out propertyValue);
                    if (hasValue)
                    {
                        outObj.GetType().GetProperty(property.Name).SetValue(outObj, Convert.ToDecimal(propertyValue), null);
                    }
                }
                else if (Nullable.GetUnderlyingType(property.PropertyType) == typeof(DateTime) | property == typeof(DateTime)) //DateTime offset to DateTime conversion
                {
                    object propertyValue = new();

                    var hasValue = tableEntity.TryGetValue(property.Name, out propertyValue);
                    if (hasValue)
                    {
                        DateTimeOffset timeOffset = (DateTimeOffset)propertyValue;
                        outObj.GetType().GetProperty(property.Name).SetValue(outObj, timeOffset.UtcDateTime, null);
                    }
                }
                else
                {
                    object propertyValue = new();

                    var hasValue = tableEntity.TryGetValue(property.Name, out propertyValue);
                    if (hasValue)
                    {
                        outObj.GetType().GetProperty(property.Name).SetValue(outObj, propertyValue, null);
                    }
                }
            }

            return outObj;
        }
        public static TableEntity ModelToTableEntity(this object input, TableClient client, string partition, string rowKey)
        { //converts input object to table entity
            return BuildEntity(partition, rowKey, input.AddUTC());
        }
    }
}