using Microsoft.SportsCloud.TableStorage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebRole.Models;

namespace WebRole.Data
{
    public class TableStorageDataProvider : IDataProvider
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

        public async Task Delete<T>(IEnumerable<T> inputs) where T : IDataModel
        {
            //var tableStorageAccessor = _tableStorageAccessors[typeof(T)];
            
            //tableStorageAccessor.DeleteEntitiesAsync<>


            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> FetchAll<T>() where T : IDataModel
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> FetchAll<T>(string groupKey) where T : IDataModel
        {
            throw new NotImplementedException();
        }

        public async Task Insert<T>(IEnumerable<T> inputs) where T : IDataModel
        {
            throw new NotImplementedException();
        }

        public async Task Update<T>(IEnumerable<T> inputs) where T : IDataModel
        {
            throw new NotImplementedException();
        }
    }
}