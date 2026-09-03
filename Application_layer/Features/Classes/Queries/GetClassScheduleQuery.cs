using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.Classes.Queries
{
    // Query Definition
    public record GetClassScheduleQuery(int? BranchId, DateTime Date) : IRequest<IEnumerable<object>>;

    // Handler Implementation
    public class GetClassScheduleQueryHandler : IRequestHandler<GetClassScheduleQuery, IEnumerable<object>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetClassScheduleQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<object>> Handle(GetClassScheduleQuery request, CancellationToken cancellationToken)
        {
            // 1. Fetch ClassSessions via Repository
            var sessions = await _unitOfWork.ClassSessions.GetAllAsync(cancellationToken);

            // 2. Convert DateTime query parameter to DateOnly for comparison
            var queryDate = DateOnly.FromDateTime(request.Date);
            sessions = sessions.Where(s => s.SessionDate == queryDate);

            // 3. Filter by BranchId if provided
            if (request.BranchId.HasValue && request.BranchId.Value > 0)
            {
                sessions = sessions.Where(s => s.BranchId == request.BranchId.Value);
            }

            // 4. Manual Mapping to Anonymous Object (Calculating EndTime dynamically)
            var result = sessions
                .OrderBy(s => s.StartTime)
                .Select(s => new
                {
                    s.Id,
                    s.ClassName,
                    s.BranchId,
                    s.StudioId,
                    s.TrainerId,
                    s.SessionDate,
                    s.StartTime,
                    EndTime = s.StartTime.Add(TimeSpan.FromMinutes(s.DurationInMinutes)),
                    s.DurationInMinutes,
                    s.CapacityLimit,
                    s.Status,
                    s.Description
                });

            return result;
        }
    }
}