using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan_Fitness.Application_layer.Features.Dashboard.Queries;

namespace Titan_Fitness.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DashboardController : ControllerBase
    {
        private readonly ISender _mediator;

        public DashboardController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// الحصول على الملخص الإحصائي للنظام
        /// </summary>
        [HttpGet("summary")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
        {
            var query = new GetDashboardSummaryQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}