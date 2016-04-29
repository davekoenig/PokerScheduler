using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebRole.Data
{
    public interface IDataProvider<T>
    {
        IEnumerable<T> FetchAll();
        IEnumerable<T> FetchAll(string groupKey);

        void Update(IEnumerable<T> inputs);

        void Insert(IEnumerable<T> inputs);

        void Delete(IEnumerable<T> inputs);
    }
}
