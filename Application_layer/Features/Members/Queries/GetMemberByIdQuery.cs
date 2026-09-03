using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.Members.Queries
{
    // Query definition returning nullable MemberDto
    public record GetMemberByIdQuery(int Id) : IRequest<MemberDto?>;

    // Handler implementation within the same Vertical Slice file
    public class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, MemberDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMemberByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MemberDto?> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Input Validation & IDOR Protection Check
            if (request.Id <= 0)
            {
                return null;
            }

            // 2. Fetch Entity from Repository
            var member = await _unitOfWork.Members.GetByIdAsync(request.Id, cancellationToken);
            if (member == null)
            {
                return null;
            }

            // 3. Map Domain Entity to MemberDto
            // Value Objects (Phone and Address) are extracted safely using ToString() or property access
            return new MemberDto
            {
                Id = member.Id,
                MembershipNumber = member.MembershipNumber,
                FullName = member.FullName,
                Email = member.Email ?? string.Empty,
                Phone = member.Phone?.ToString() ?? string.Empty,
                Address = member.Address?.ToString() ?? string.Empty,
                JoinedDate = member.JoinedDate,
                HomeBranchId = member.HomeBranchId,
                HomeBranchName = string.Empty, // Populate via Branch lookup if needed
                Status = "Active",             // Managed via membership business logic
                LastVisit = null
            };
        }
    }
}