using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.Trainers.Queries
{
    // Query Definition
    public record GetTrainerByIdQuery(int Id) : IRequest<object?>;

    // Handler Implementation
    public class GetTrainerByIdQueryHandler : IRequestHandler<GetTrainerByIdQuery, object?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTrainerByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<object?> Handle(GetTrainerByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Fetch Trainer entity from repository
            var trainer = await _unitOfWork.Trainers.GetByIdAsync(request.Id, cancellationToken);

            if (trainer == null)
            {
                return null;
            }

            // 2. Manual Mapping to Anonymous Object
            return new
            {
                trainer.Id,
                FullName = trainer.Name,
                trainer.Email,
                Phone = trainer.Phone?.Value, // Extract string value from Phone Value Object
                trainer.IsActive
            };
        }
    }
}