using EventSourcingPoc.API.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingPoc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuaranteesController : ControllerBase
    {
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<GuaranteesController> _logger;
        private readonly CreateGuaranteeHandler _createGuaranteeHandler;
        private readonly ConfirmGuaranteePriceHandler _confirmGuaranteePriceHandler;
        private readonly UpdateGuaranteeInformationHandler _updateGuaranteeInformationHandler;
        private readonly IssueGuaranteeHandler _issueGuaranteeHandler;

        public GuaranteesController(
            CreateGuaranteeHandler createGuaranteeHandler, 
            ConfirmGuaranteePriceHandler confirmGuaranteePriceHandler, 
            UpdateGuaranteeInformationHandler updateGuaranteeInformationHandler,
            IssueGuaranteeHandler issueGuaranteeHandler,
            ILogger<GuaranteesController> logger, 
            TimeProvider timeProvider)
        {
            _createGuaranteeHandler = createGuaranteeHandler;
            _confirmGuaranteePriceHandler = confirmGuaranteePriceHandler;
            _issueGuaranteeHandler = issueGuaranteeHandler;
            _updateGuaranteeInformationHandler = updateGuaranteeInformationHandler;
            _logger = logger;
            _timeProvider = timeProvider;
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

        [HttpPost("{id}/issue")]
        public async Task<ActionResult> IssueGuaranteeAsync(Guid id, CancellationToken cancellationToken)
        {
            IssueGuaranteeCommand command = new(id, _timeProvider.GetUtcNow().UtcDateTime);
            await _issueGuaranteeHandler.Handle(command, cancellationToken);
            return Ok();
        }


        /// <summary>
        /// Patch guarantee information
        /// </summary>
        /// <param name="id">Guarantee ID</param>
        /// <param name="payload">Payload with information to update</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        [HttpPatch("{id}/info")]
        public async Task<ActionResult> PatchGuaranteeInformation(Guid id, PatchGuaranteeInfoPayload payload, CancellationToken cancellationToken)
        {
            UpdateGuaranteeInformationCommand updateGuaranteeInformationCommand = new(
                Id: id,
                Gloss: payload.Gloss,
                TenderId: payload.TenderId,
                SupplierInfo: payload.Supplier != null 
                    ? new UpdateGuaranteePartyInformation(payload.Supplier.Name, payload.Supplier.AddressStreet, payload.Supplier.AddressLocation, payload.Supplier.AddressRegion) 
                    :null,
                BeneficiaryInfo: payload.Beneficiary != null
                    ? new UpdateGuaranteePartyInformation(payload.Beneficiary.Name, payload.Beneficiary.AddressStreet, payload.Beneficiary.AddressLocation, payload.Beneficiary.AddressRegion)
                    : null
            );

            var response = await _updateGuaranteeInformationHandler.Handle(updateGuaranteeInformationCommand, cancellationToken);
            if (!response.Success)
                return Problem(
                    title: "Ha ocurrido un error al actualizar datos de la garantia",
                    detail: response.Error,
                    statusCode: StatusCodes.Status422UnprocessableEntity
                );

            return Ok(response.Value);
        }

    }

    public record PatchGuaranteeInfoPayload(
        string? Gloss,
        string? TenderId,
        PatchGuaranteeLegalParty? Beneficiary,
        PatchGuaranteeLegalParty? Supplier
    );

    public record PatchGuaranteeLegalParty(string? Name, string? AddressStreet, string? AddressLocation, string? AddressRegion);

    public record ConfirmGuaranteeCostPayload(decimal Cost);
}
