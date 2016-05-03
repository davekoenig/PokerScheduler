using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using WebRole.Models;

namespace WebRole.Data
{
    public class TableStorageDataProvider : IDataProvider
    {
        public void Delete<T>(IEnumerable<T> inputs) where T : IDataModel
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FetchAll<T>() where T : IDataModel
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FetchAll<T>(string groupKey) where T : IDataModel
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(IEnumerable<T> inputs) where T : IDataModel
        {
            throw new NotImplementedException();
        }

        public void Update<T>(IEnumerable<T> inputs) where T : IDataModel
        {
            throw new NotImplementedException();
        }
    }
}