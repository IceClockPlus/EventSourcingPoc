using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingPoc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IDocumentStore _documentStore;
        public ProductsController(IDocumentStore documentStore)
        {

            _documentStore = documentStore;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            await using var session = _documentStore.QuerySession();
            var product = await session.Events.AggregateStreamAsync<Product>(id);
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] CreateProductPayload payload)
        {
            await using var session = _documentStore.LightweightSession();
            Guid id = Guid.NewGuid();
            session.Events.Append(id, new ProductCreated(Id: id, Name: payload.Name, Price: payload.Price, Quantity: payload.Quantity));
            await session.SaveChangesAsync();
            return Ok();
        }

        [HttpPatch("{id}/quantity/add")]
        public async Task<ActionResult> IncreaseProductQuantity(Guid id, [FromBody] IncreaseDecreaseQuantityPayload payload)
        {
            await using var session = _documentStore.LightweightSession();
            session.Events.Append(id, new ProductAdded(Quantity: payload.Quantity));
            await session.SaveChangesAsync();
            return Ok();
        }

        [HttpPatch("{id}/quantity/delete")]
        public async Task<ActionResult> DecreaseProductQuantity(Guid id, [FromBody] IncreaseDecreaseQuantityPayload payload)
        {
            await using var session = _documentStore.LightweightSession();
            session.Events.Append(id, new ProductDeleted(Quantity: payload.Quantity));
            await session.SaveChangesAsync();
            return Ok();
        }
    }

    public record IncreaseDecreaseQuantityPayload(int Quantity);

    public record CreateProductPayload(string Name, decimal Price, int Quantity);
}
