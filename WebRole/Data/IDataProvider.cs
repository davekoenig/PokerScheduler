using System.Collections.Generic;
using System.Threading.Tasks;
using WebRole.Models;

namespace WebRole.Data
{
    public interface IDataProvider
    {
        IEnumerable<T> FetchAll<T>() where T : IDataModel;

        IEnumerable<T> FetchAll<T>(string groupKey) where T : IDataModel;

        void Update<T>(IEnumerable<T> inputs) where T : IDataModel;

        void Insert<T>(IEnumerable<T> inputs) where T : IDataModel;

        void Delete<T>(IEnumerable<T> inputs) where T : IDataModel;
    }
}
