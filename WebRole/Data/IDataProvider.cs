using System.Collections.Generic;
using System.Threading.Tasks;
using WebRole.Models;

namespace WebRole.Data
{
    public interface IDataProvider
    {
        Task<IEnumerable<T>> FetchAll<T>() where T : IDataModel;

        Task<IEnumerable<T>> FetchAll<T>(string groupKey) where T : IDataModel;

        Task Update<T>(IEnumerable<T> inputs) where T : IDataModel;

        Task Insert<T>(IEnumerable<T> inputs) where T : IDataModel;

        Task Delete<T>(IEnumerable<T> inputs) where T : IDataModel;
    }
}
