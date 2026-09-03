using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Application_layer.Features.Plans.Commands;
using Titan_Fitness.Application_layer.Features.Plans.Queries;

namespace Titan_Fitness.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PlansController : ControllerBase
    {
        private readonly ISender _mediator;

        public PlansController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// استرجاع خطط الاشتراكات
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPlans(
            [FromQuery] string? searchTerm,
            [FromQuery] int? branchId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetPlansQuery(searchTerm, branchId, pageNumber, pageSize);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// استرجاع خطة اشتراك بواسطة الـ ID
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlanById(int id, CancellationToken cancellationToken)
        {
            var query = new GetPlanByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// إضافة خطة اشتراك جديدة
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePlan(
            [FromBody] CreatePlanDto dto,
            CancellationToken cancellationToken)
        {
            var command = new CreatePlanCommand(dto);
            var id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetPlanById), new { id }, new { id });
        }

        /// <summary>
        /// تعديل بيانات خطة اشتراك
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePlan(
            int id,
            [FromBody] CreatePlanDto dto,
            CancellationToken cancellationToken)
        {
            var command = new UpdatePlanCommand(id, dto);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}