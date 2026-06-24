using EventSourcingPoc.API.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingPoc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuaranteesController : ControllerBase
    {
        private readonly ILogger<GuaranteesController> _logger;
        private readonly CreateGuaranteeHandler _createGuaranteeHandler;
        private readonly ConfirmGuaranteePriceHandler _confirmGuaranteePriceHandler;
        public GuaranteesController(CreateGuaranteeHandler createGuaranteeHandler, ConfirmGuaranteePriceHandler confirmGuaranteePriceHandler, ILogger<GuaranteesController> logger)
        {
            _createGuaranteeHandler = createGuaranteeHandler;
            _confirmGuaranteePriceHandler = confirmGuaranteePriceHandler;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> CreateGuarantee(CreateGuaranteeCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _createGuaranteeHandler.Handle(command, cancellationToken);
                if (!response.Success)
                {
                    return Problem(
                        title: "Error al solicitar garantia",
                        detail: response.Error,
                        statusCode: StatusCodes.Status422UnprocessableEntity
                    );
                }
                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened when requesting a guarantee: {message}", ex.Message);
                return Problem(
                    title: "Error interno del servidor",
                    detail: "Ha ocurrido un error inesperado al procesar su solicitud",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }        
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
