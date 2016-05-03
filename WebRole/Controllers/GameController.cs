using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebRole.Data;
using WebRole.Models;

namespace WebRole.Controllers
{
    public class GameController : ApiController
    {
        [HttpGet]
        // GET: api/Game
        public async Task<IEnumerable<IGame>> Get()
        {
            var games = await DataManager.Instance.Provider.FetchAll<IGame>();

            return games;
        }

        [HttpGet]
        // GET: api/Game/5
        public async Task<IGame> Get(int id)
        {
            var games = await Get();

            return games.FirstOrDefault(g => g.GameId.Equals(id));
        }

        [HttpPost]
        // POST: api/Game
        public async Task Post([FromBody]IGame value)
        {
            await DataManager.Instance.Provider.Insert<IGame>(new[] { value });
        }

        [HttpPut]
        // PUT: api/Game/5
        public async Task Put([FromBody]IGame value)
        {
            await DataManager.Instance.Provider.Update<IGame>(new[] { value });
        }

        [HttpDelete]
        // DELETE: api/Game/5
        public async Task Delete(int id)
        {
            var game = await Get(id);

            await DataManager.Instance.Provider.Delete<IGame>(new[] { game });
        }
    }
}
