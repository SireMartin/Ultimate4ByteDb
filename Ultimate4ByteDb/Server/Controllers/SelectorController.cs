using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        public ActionResult<Shared.FourByteSelector> GetSelectorInfo([FromRoute]string argSelector)
        {
            string selector = argSelector.ToLower().StartsWith("0x") ? argSelector.Substring(2) : argSelector;
            RedisValue redisValue = _db.StringGet(selector.ToLower());
            if (redisValue.IsNull)
            {
                return NotFound();
            }
            string content = Encoding.UTF8.GetString(redisValue!);
            //hier zelf de opties specifieren, want dit valt niet onder de controller configuratie
            Shared.FourByteSelector fbs = JsonSerializer.Deserialize<Shared.FourByteSelector>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } })!;
            //door de configuratie in de program.cs gebeurt hier automatisch hetzelfde als in commentiaar onder (camelcase is wel default)
            return fbs;
            //return new JsonResult(fbs, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Converters = { new JsonStringEnumConverter() } });
        }
    }
}
