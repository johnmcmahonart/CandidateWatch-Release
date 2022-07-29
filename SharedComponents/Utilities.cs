using Azure.Data.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
namespace FECIngest
{
    public static class Utilities
    //fixes dates in FECAPI not having UTC specified
    {
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
                else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(Decimal) | prop.PropertyType == typeof(long) & prop.GetValue(source) != null)
                {
                    tableEntity.Add(prop.Name, prop.GetValue(source));
                }
                else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(string) | prop.PropertyType == typeof(string) & prop.GetValue(source) != null)
                {
                    tableEntity.Add(prop.Name, prop.GetValue(source));
                }
                else if (prop.GetValue(source) !=null) //is complex type, so we Serialize the object to json and store as string
                {
                    //need to change default serialization settings to prevent self reference looping
                    //this may no longer be necessary as this was added because of a bug in a previous version
                    string jsonString = JsonConvert.SerializeObject(prop.GetValue(source),Formatting.None, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    tableEntity.Add(prop.Name+"-json", jsonString); //add -json to column name so when we read the data back out we know to deserialize the string to json
                }
            }
            return tableEntity;
        }
        public static TableEntity ToTable(this object input, TableClient client, string partition, string rowKey)
        //converts input object to table entity
        {
            return BuildEntity(partition, rowKey, input);
        }

            public static string GetMemberName<T, TValue>(Expression<Func<T, TValue>> memberAccess)
        //https://stackoverflow.com/questions/7598968/getting-the-name-of-a-property-in-c-sharp

        {
            return ((MemberExpression)memberAccess.Body).Member.Name;
        }

    }
}