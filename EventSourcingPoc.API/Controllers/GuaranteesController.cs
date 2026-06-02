using EventSourcingPoc.API.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingPoc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuaranteesController : ControllerBase
    {
        private readonly CreateGuaranteeHandler _createGuaranteeHandler;
        private readonly ConfirmGuaranteePriceHandler _confirmGuaranteePriceHandler;
        public GuaranteesController(CreateGuaranteeHandler createGuaranteeHandler, ConfirmGuaranteePriceHandler confirmGuaranteePriceHandler)
        {
            _createGuaranteeHandler = createGuaranteeHandler;
            _confirmGuaranteePriceHandler = confirmGuaranteePriceHandler;
        }

        [HttpPost]
        public async Task<ActionResult> CreateGuarantee(CreateGuaranteeCommand command)
        {
            await _createGuaranteeHandler.Handle(command, CancellationToken.None);
            return Ok();
        }

        [HttpPost("{id}/cost")]
        public async Task<ActionResult> ConfirmGuaranteeCost(Guid id, ConfirmGuaranteeCostPayload payload)
        {
            ConfirmGuaranteePriceCommand command = new(id, payload.Cost);
            await _confirmGuaranteePriceHandler.Handle(command, CancellationToken.None);
            return Ok();
        }
    }

    public record ConfirmGuaranteeCostPayload(decimal Cost);
}
