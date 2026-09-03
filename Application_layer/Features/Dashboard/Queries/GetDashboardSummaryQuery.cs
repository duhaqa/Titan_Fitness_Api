using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.Dashboard.Queries
{
    // Query Definition
    public record GetDashboardSummaryQuery : IRequest<object>;

    // Handler Implementation
    public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, object>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDashboardSummaryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<object> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
        {
            // 1. Fetch data aggregates parallelly or sequentially via UnitOfWork repositories
            var members = await _unitOfWork.Members.GetAllAsync(cancellationToken);
            var trainers = await _unitOfWork.Trainers.GetAllAsync(cancellationToken);
            var plans = await _unitOfWork.Plans.GetAllAsync(cancellationToken);

            // 2. Aggregate counts and key metrics
            var totalMembers = members.Count();
            var totalTrainers = trainers.Count();
            var activeTrainers = trainers.Count(t => t.IsActive);
            var totalPlans = plans.Count();
            var publishedPlans = plans.Count(p => p.IsPublished);

            // 3. Return aggregated summary response
            return new
            {
                TotalMembers = totalMembers,
                TotalTrainers = totalTrainers,
                ActiveTrainers = activeTrainers,
                TotalPlans = totalPlans,
                PublishedPlans = publishedPlans
            };
        }
    }
}