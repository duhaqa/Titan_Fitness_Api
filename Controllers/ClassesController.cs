using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Application_layer.Features.Classes.Commands;
using Titan_Fitness.Application_layer.Features.Classes.Queries;

namespace Titan_Fitness.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ClassesController : ControllerBase
    {
        private readonly ISender _mediator;

        public ClassesController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// استرجاع جدول الحصص الرياضية حسب الفرع والتاريخ
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Receptionist,Trainer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSchedule(
            [FromQuery] int? branchId,
            [FromQuery] DateTime? date,
            CancellationToken cancellationToken)
        {
            var query = new GetClassScheduleQuery(branchId, date ?? DateTime.Today);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// إنشاء حصة تدريبية جديدة
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateClassSession(
            [FromBody] CreateClassSessionDto dto,
            CancellationToken cancellationToken)
        {
            var command = new CreateClassSessionCommand(dto);
            var id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetSchedule), new { id }, new { id });
        }

        /// <summary>
        /// حجز موعد في حصة تدريبية للمشترك
        /// </summary>
        [HttpPost("{id:int}/book")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BookSession(
            int id,
            [FromBody] BookSessionDto dto,
            CancellationToken cancellationToken)
        {
            if (id != dto.SessionId)
                return BadRequest("معرف الحصة غير متطابق مع بيانات الطلب.");

            var command = new BookSessionCommand(dto);
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }
    }
}