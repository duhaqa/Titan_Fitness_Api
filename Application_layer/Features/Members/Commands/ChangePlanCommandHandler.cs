using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Interfaces;
using Titan_Fitness.Domain.Entites;


namespace Titan_Fitness.Application_layer.Features.Members.Commands
{
    // Command definition
    public record ChangePlanCommand(ChangePlanDto ChangePlanDto) : IRequest<bool>;

    // Handler implementation within the same feature file
    public class ChangePlanCommandHandler : IRequestHandler<ChangePlanCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangePlanCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ChangePlanCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate request payload existence
            if (request?.ChangePlanDto == null)
            {
                throw new ArgumentNullException(nameof(request.ChangePlanDto), "Request payload cannot be null.");
            }

            var dto = request.ChangePlanDto;

            // 2. Validate essential identifiers
            if (dto.MemberId <= 0 || dto.NewPlanId <= 0)
            {
                throw new ArgumentException("Invalid Member ID or Plan ID specified.");
            }

            // 3. IDOR and existence check for Member entity
            var member = await _unitOfWork.Members.GetByIdAsync(dto.MemberId, cancellationToken);
            if (member == null)
            {
                throw new KeyNotFoundException($"Member with ID {dto.MemberId} was not found.");
            }

            // 4. Validate existence and availability of the target plan
            var newPlan = await _unitOfWork.Plans.GetByIdAsync(dto.NewPlanId, cancellationToken);
            if (newPlan == null || !newPlan.IsPublished)
            {
                throw new InvalidOperationException("Selected plan is inactive or unavailable.");
            }

            // 5. Save changes using UnitOfWork
            return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}