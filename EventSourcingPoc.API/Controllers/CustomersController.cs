using EventSourcingPoc.API.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingPoc.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController(ICommandDispatcher commands) : ControllerBase
    {
        private readonly ICommandDispatcher _commands = commands;

        [HttpPost]
        public async Task<IActionResult> RegisterCustomer([FromBody] Contracts.RegisterCustomerPayload payload, CancellationToken cancellationToken)
        {
            var command = new RegisterCustomeCommand(payload.TaxId, payload.Name);
            var result = await _commands.Dispatch(command, cancellationToken);
            return Ok(result);
        }
    }
}