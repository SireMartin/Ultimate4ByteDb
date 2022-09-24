using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace Ultimate4ByteDb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SelectorController : ControllerBase
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public SelectorController() 
        {
            _redis = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false,ssl=false,allowAdmin=true");
            _db = _redis.GetDatabase();
        }

        [HttpGet]
        [Route("{argSelector}")]
        public ActionResult<string> GetSelectorInfo([FromRoute]string argSelector)
        {
            string content = Encoding.UTF8.GetString(_db.StringGet(argSelector));
            Shared.FourByteSelector fbs = JsonSerializer.Deserialize<Shared.FourByteSelector>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok();
        }
    }
}
