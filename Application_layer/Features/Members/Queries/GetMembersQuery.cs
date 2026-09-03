using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.Members.Queries
{
    // Query definition with pagination and filtering parameters
    public record GetMembersQuery(
        string? SearchTerm,
        int? BranchId,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<IEnumerable<MemberDto>>;

    // Handler implementation within the same Vertical Slice file
    public class GetMembersQueryHandler : IRequestHandler<GetMembersQuery, IEnumerable<MemberDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMembersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MemberDto>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
        {
            // 1. Fetch members from repository (This signature will be added to IMemberRepository later)
            var members = await _unitOfWork.Members.GetAllAsync(cancellationToken);

            if (members == null)
            {
                return Enumerable.Empty<MemberDto>();
            }

            var query = members.AsQueryable();

            // 2. Search Filter (by FullName or MembershipNumber)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(m =>
                    (m.FullName != null && m.FullName.ToLower().Contains(term)) ||
                    (m.MembershipNumber != null && m.MembershipNumber.ToLower().Contains(term)));
            }

            // 3. Branch Filter
            if (request.BranchId.HasValue && request.BranchId.Value > 0)
            {
                query = query.Where(m => m.HomeBranchId == request.BranchId.Value);
            }

            // 4. Pagination Logic
            int pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            int pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var pagedMembers = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 5. Safe Manual Mapping to MemberDto
            return pagedMembers.Select(member => new MemberDto
            {
                Id = member.Id,
                MembershipNumber = member.MembershipNumber,
                FullName = member.FullName,
                Email = member.Email ?? string.Empty,
                Phone = member.Phone != null ? member.Phone.Value : string.Empty,
                Address = member.Address != null ? member.Address.Value : string.Empty,
                JoinedDate = member.JoinedDate,
                HomeBranchId = member.HomeBranchId,
                HomeBranchName = string.Empty,
                Status = "Active",
                LastVisit = null
            }).ToList();
        }
    }
}