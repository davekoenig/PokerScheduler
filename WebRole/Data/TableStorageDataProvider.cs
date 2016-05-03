using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebRole.Models;

namespace WebRole.Data
{
    public class TableStorageDataProvider : IDataProvider
    {
        public async Task Delete<T>(IEnumerable<T> inputs) where T : IDataModel
        {
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