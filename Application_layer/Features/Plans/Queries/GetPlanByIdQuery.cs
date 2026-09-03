using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.Plans.Queries
{
    // Query Definition
    public record GetPlanByIdQuery(int Id) : IRequest<object?>;

    // Handler Implementation
    public class GetPlanByIdQueryHandler : IRequestHandler<GetPlanByIdQuery, object?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPlanByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<object?> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Fetch Plan entity using Generic Repository
            var plan = await _unitOfWork.Plans.GetByIdAsync(request.Id, cancellationToken);

            if (plan == null)
            {
                return null;
            }

            // 2. Manual Mapping to Anonymous Object / DTO to enforce clean separation
            return new
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
            };
        }
    }
}