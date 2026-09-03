using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Application_layer.Features.CheckIn.Commands;

namespace Titan_Fitness.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CheckInController : ControllerBase
    {
        private readonly ISender _mediator;

        public CheckInController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// معالجة تسجيل دخول المشترك بالصالة الرياضية
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProcessCheckIn(
            [FromBody] ProcessCheckInDto dto,
            CancellationToken cancellationToken)
        {
            var command = new ProcessCheckInCommand(dto);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}