using EventSourcingPoc.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingPoc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BondsController : ControllerBase
    {
        private readonly IBondsService _bondsService;
        public BondsController(IBondsService bondsService)
        {
            _bondsService = bondsService;
        }

        [HttpGet]
        public async Task<ActionResult> GetBonds(CancellationToken cancellationToken)
        {
            var bonds = await _bondsService.GetAllBonds(cancellationToken);
            return Ok(bonds);
        }
    }
}
