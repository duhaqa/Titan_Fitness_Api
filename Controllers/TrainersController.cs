using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Application_layer.Features.Trainers.Commands;
using Titan_Fitness.Application_layer.Features.Trainers.Queries;

namespace Titan_Fitness.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TrainersController : ControllerBase
    {
        private readonly ISender _mediator;

        public TrainersController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// الحصول على قائمة المدربين
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTrainers(
            [FromQuery] string? searchTerm,
            [FromQuery] int? branchId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetTrainersQuery(searchTerm, branchId, pageNumber, pageSize);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// استرجاع بيانات مدرب محدد بواسطة الـ ID
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTrainerById(int id, CancellationToken cancellationToken)
        {
            var query = new GetTrainerByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// إضافة مدرب جديد
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTrainer(
            [FromBody] CreateTrainerDto dto,
            CancellationToken cancellationToken)
        {
            var command = new CreateTrainerCommand(dto);
            var id = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetTrainerById), new { id }, new { id });
        }

        /// <summary>
        /// تحديث بيانات المدرب
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateTrainer(
            int id,
            [FromBody] CreateTrainerDto dto,
            CancellationToken cancellationToken)
        {
            var command = new UpdateTrainerCommand(id, dto);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}