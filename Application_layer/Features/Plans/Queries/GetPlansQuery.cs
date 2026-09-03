using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.Plans.Queries
{
    // Query Definition
    public record GetPlansQuery(
        string? SearchTerm,
        int? BranchId,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<IEnumerable<object>>;

    // Handler Implementation
    public class GetPlansQueryHandler : IRequestHandler<GetPlansQuery, IEnumerable<object>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPlansQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<object>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
        {
            // 1. Fetch all plans using Generic Repository
            var plans = await _unitOfWork.Plans.GetAllAsync(cancellationToken);

            // 2. Apply Search Filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                plans = plans.Where(p => p.Name.ToLower().Contains(term));
            }

            // 3. Apply Pagination and Manual Mapping
            var result = plans
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(plan => new
                {
                    plan.Id,
                    plan.Name,
                    plan.Price,
                    plan.DurationInMonths,
                    plan.MaxFreezeDays,
                    plan.MaxNumberOfFreezes,
                    plan.GuestPassQuota,
                    plan.AccessScope,
                    plan.IsPublished
                });

            return result;
        }
    }
}