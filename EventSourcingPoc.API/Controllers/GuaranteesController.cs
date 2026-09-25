using Microsoft.AspNetCore.Mvc;

namespace EventSourcingPoc.API.Controllers
{
    [Route("api/[controller]")]
    public class GuaranteesController : ControllerBase
    {
        [HttpPost]
        public IActionResult PostGuarantee([FromBody] Contracts.RegisterGuaranteePayload guaranteeRequest)
        {
            // Handle the guarantee request here
            
            return Ok();
        }        
    }
}