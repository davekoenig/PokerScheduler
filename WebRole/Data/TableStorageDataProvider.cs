using Microsoft.SportsCloud.TableStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebRole.Models;

namespace WebRole.Data
{
    public class TableStorageDataProvider //: IDataProvider
    {
        private Dictionary<Type, TableStorageAccessor> _tableStorageAccessors = new Dictionary<Type, TableStorageAccessor>();

        public TableStorageDataProvider(string connectionString)
        {
            Setup(connectionString);
        }

        private void Setup(string connectionString)
        {
            var supportedTypes = StorageMapping.GetSupportedStorageTypes();
            foreach (var type in supportedTypes)
            {
                var tableName = StorageMapping.GetStorageLocation(type);
                _tableStorageAccessors[type] = new TableStorageAccessor(connectionString, tableName);
            }
        }

        private TableStorageAccessor GetAccessor<T>()
        {
            return _tableStorageAccessors[typeof(T)];
        }

        public async Task Delete<T>(IEnumerable<T> inputs) where T : TableEntity, new()
        {
            var accessor = GetAccessor<T>();

            await accessor.DeleteEntitiesBatchAsync<T>(inputs.ToList());
        }

        public async Task<IEnumerable<T>> FetchAll<T>() where T : TableEntity, new()
        {
            var accessor = GetAccessor<T>();

            return await accessor.RetrieveEntitiesAsync<T>(null);
        }

        public async Task<IEnumerable<T>> FetchAll<T>(string partitionKey) where T : TableEntity, new()
        {
            var accessor = GetAccessor<T>();

            return await accessor.RetrieveEntitiesAsync<T>(partitionKey);
        }

        public async Task Insert<T>(IEnumerable<T> inputs) where T : TableEntity, new()
        {
            var accessor = GetAccessor<T>();

            //TODO: this will stomp all entities that already exist :(
            await accessor.InsertOrReplaceEntitiesBatchAsync<T>(inputs.ToList());
        }

        public async Task Update<T>(IEnumerable<T> inputs) where T : TableEntity, new()
        {
            var accessor = GetAccessor<T>();

            await accessor.ReplaceEntitiesBatchAsync<T>(inputs.ToList());
        }
    }
}