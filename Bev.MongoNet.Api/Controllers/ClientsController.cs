using Bev.MongoNet.Mongo;
using Bev.MongoNet.Mongo.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bev.MongoNet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IMongoAdapter _mongoAdapter;

        public ClientsController(IMongoAdapter mongoAdapter)
        {
            _mongoAdapter = mongoAdapter;
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            var client = await _mongoAdapter.GetById<Client>(id, cancellationToken);

            return Ok(client);
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var clients = await _mongoAdapter.Get<Client>("{}", cancellationToken);

            return Ok(clients);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var client = new Client
            {
                Id = Guid.NewGuid().ToString().Substring(1, 10),
                Name = "Bevila",
                Salary = 10,
                CreatedAt = DateTime.Now
            };

            await _mongoAdapter.Save(client, cancellationToken);

            return Ok();
        }
    }
}
