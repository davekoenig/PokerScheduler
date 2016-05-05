using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using WebRole.Models;
using WebRole.Data;
using Microsoft.WindowsAzure.Storage.Table;

namespace WebRole.Controllers
{
    public abstract class DataModelController<T> : ApiController where T : TableEntity, IDataModel, new()
    {
        [HttpGet]
        // GET: api/DataModel
        public async Task<HttpResponseMessage> Get()
        {
            var entries = await DataManager.Instance.Provider.FetchAll<T>();

            var responseMessage = Request.CreateResponse<IEnumerable<T>>(HttpStatusCode.OK, entries);

            return responseMessage;
        }

        [HttpGet]
        // GET: api/DataModel/5
        public async Task<HttpResponseMessage> Get(string id)
        {
            var response = await Get();

            var entries = await response.Content.ReadAsAsync<IEnumerable<T>>();

            var entry = entries.FirstOrDefault<T>(x => x.Id.Equals(id));

            var responseMessage = Request.CreateResponse<T>(HttpStatusCode.OK, entry);

            return responseMessage;
        }

        [HttpPost]
        // POST: api/DataModel
        public async Task<HttpResponseMessage> Post([FromBody]T value)
        {
            await DataManager.Instance.Provider.Insert<T>(new[] { value });

            var responseMessage = Request.CreateResponse(HttpStatusCode.OK);
            return responseMessage;
        }

        [HttpPut]
        // PUT: api/DataModel/5
        public async Task<HttpResponseMessage> Put([FromBody]T value)
        {
            await DataManager.Instance.Provider.Update<T>(new[] { value });

            var responseMessage = Request.CreateResponse(HttpStatusCode.OK);

            return responseMessage;
        }

        [HttpDelete]
        // DELETE: api/DataModel/5
        public async Task<HttpResponseMessage> Delete(string id)
        {
            var response = await Get(id);

            var entry = await response.Content.ReadAsAsync<T>();

            await DataManager.Instance.Provider.Delete<T>(new[] { entry });

            var responseMessage = Request.CreateResponse(HttpStatusCode.OK);

            return responseMessage;
        }
    }
}
