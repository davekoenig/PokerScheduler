using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using WebRole.Models;
using WebRole.Data;

namespace WebRole.Controllers
{
    public abstract class DataModelController<T> : ApiController where T : IDataModel
    {
        [HttpGet]
        // GET: api/DataModel
        public async Task<IEnumerable<T>> Get()
        {
            var entries = await DataManager.Instance.Provider.FetchAll<T>();

            return entries;
        }

        [HttpGet]
        // GET: api/DataModel/5
        public async Task<T> Get(string id)
        {
            var entries = await Get();

            var entry = entries.FirstOrDefault<T>(x => x.Id.Equals(id));

            return entry;
        }

        [HttpPost]
        // POST: api/DataModel
        public async Task Post([FromBody]T value)
        {
            await DataManager.Instance.Provider.Insert<T>(new[] { value });
        }

        [HttpPut]
        // PUT: api/DataModel/5
        public async Task Put([FromBody]T value)
        {
            await DataManager.Instance.Provider.Update<T>(new[] { value });
        }

        [HttpDelete]
        // DELETE: api/DataModel/5
        public async Task Delete(string id)
        {
            var entry = await Get(id);

            await DataManager.Instance.Provider.Delete<T>(new[] { entry });
        }
    }
}
