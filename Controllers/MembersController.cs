using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Application_layer.Features.Members.Commands;
using Titan_Fitness.Application_layer.Features.Members.Queries;

namespace Titan_Fitness.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MembersController : ControllerBase
    {
        private readonly ISender _mediator;

        public MembersController(ISender mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// استعلام واسترجاع المشتركين بأسلوب الصفحات (Pagination)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMembers(
            [FromQuery] string? searchTerm,
            [FromQuery] int? branchId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetMembersQuery(searchTerm, branchId, pageNumber, pageSize);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على بيانات مشترك محدد بواسطة الـ ID
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Receptionist,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMemberById(int id, CancellationToken cancellationToken)
        {
            var query = new GetMemberByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// تسجيل مشترك جديد
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist,Manager")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMember(
            [FromBody] CreateMemberDto dto,
            CancellationToken cancellationToken)
        {
            var command = new CreateMemberCommand(dto);
            var createdMemberId = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(
                nameof(GetMemberById),
                new { id = createdMemberId },
                new { id = createdMemberId });
        }

        /// <summary>
        /// تحديث بيانات المشترك
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMember(
            int id,
            [FromBody] UpdateMemberDto dto,
            CancellationToken cancellationToken)
        {
            if (id != dto.Id)
                return BadRequest("معرف المشترك غير متطابق مع البيانات الإضافية.");

            var command = new UpdateMemberCommand(dto);
            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// تغيير أو تجديد خطة اشتراك المشترك
        /// </summary>
        [HttpPost("{id:int}/change-plan")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePlan(
            int id,
            [FromBody] ChangePlanDto dto,
            CancellationToken cancellationToken)
        {
            if (id != dto.MemberId)
                return BadRequest("معرف المشترك غير متطابق مع بيانات الطلب.");

            var command = new ChangePlanCommand(dto);
            await _mediator.Send(command, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// تجميد اشتراك المشترك
        /// </summary>
        [HttpPost("{id:int}/freeze")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FreezeMembership(
            int id,
            [FromBody] FreezeMembershipDto dto,
            CancellationToken cancellationToken)
        {
            if (id != dto.MemberId)
                return BadRequest("معرف المشترك غير متطابق مع بيانات الطلب.");

            var command = new FreezeMembershipCommand(dto);
            await _mediator.Send(command, cancellationToken);

            return Ok();
        }
    }
}