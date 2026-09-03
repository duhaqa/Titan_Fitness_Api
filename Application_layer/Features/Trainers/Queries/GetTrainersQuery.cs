using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.Trainers.Queries
{
    // Query Definition
    public record GetTrainersQuery(
        string? SearchTerm,
        int? BranchId,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<IEnumerable<object>>;

    // Handler Implementation
    public class GetTrainersQueryHandler : IRequestHandler<GetTrainersQuery, IEnumerable<object>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTrainersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<object>> Handle(GetTrainersQuery request, CancellationToken cancellationToken)
        {
            // 1. Fetch trainers from repository
            var trainers = await _unitOfWork.Trainers.GetAllAsync(cancellationToken);

            // 2. Apply Search Filter if SearchTerm is provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                trainers = trainers.Where(t =>
                    t.Name.ToLower().Contains(term) ||
                    (t.Email != null && t.Email.ToLower().Contains(term))
                );
            }

            // 3. Apply Pagination and Manual Mapping
            var result = trainers
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(trainer => new
                {
                    trainer.Id,
                    FullName = trainer.Name,
                    trainer.Email,
                    Phone = trainer.Phone?.Value,
                    trainer.IsActive
                });

            return result;
        }
    }
}