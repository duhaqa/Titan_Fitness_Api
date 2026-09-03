using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.Plans.Commands
{
    // Command Record Definition
    public record CreatePlanCommand(CreatePlanDto PlanDto) : IRequest<int>;

    // Handler Implementation
    public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        // Constructor ensuring _unitOfWork is initialized (Fixes CS8618)
        public CreatePlanCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<int> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
        {
            if (request.PlanDto == null)
            {
                throw new ArgumentNullException(nameof(request.PlanDto), "بيانات الخطة مطلوبة.");
            }

            var dto = request.PlanDto;

            // Mapping exact parameters matching Plan.Create static method signature
            var plan = Plan.Create(
                name: dto.PlanName,
                price: dto.Price,
                durationInMonths: dto.DurationInMonths,
                maxFreezeDays: dto.MaxFreezeDays,
                maxNumberOfFreezes: dto.MaxNumberOfFreezes,
                guestPassQuota: dto.GuestPassQuota,
                accessScope: dto.AccessScope,
                isPublished: dto.IsPublished
            );

            // Save using Generic Repository within UnitOfWork
            await _unitOfWork.Plans.AddAsync(plan, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return plan.Id;
        }
    }
}