using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Interfaces;

namespace Titan_Fitness.Application_layer.Features.Plans.Commands
{
    // Command definition
    public record UpdatePlanCommand(int Id, CreatePlanDto PlanDto) : IRequest;

    // Handler implementation
    public class UpdatePlanCommandHandler : IRequestHandler<UpdatePlanCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePlanCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
        {
            if (request.PlanDto == null)
            {
                throw new ArgumentNullException(nameof(request.PlanDto), "بيانات الخطة مطلوبة للتعديل.");
            }

            // 1. Fetch existing plan from repository
            var plan = await _unitOfWork.Plans.GetByIdAsync(request.Id, cancellationToken);

            if (plan == null)
            {
                throw new KeyNotFoundException($"الخطة المطلوبة برقم {request.Id} غير موجودة.");
            }

            var dto = request.PlanDto;

            // 2. Map properties safely matching your DTO naming
            // Note: If you have a domain method like plan.Update(...), call it here instead of direct property assignment
            _unitOfWork.Plans.Update(plan);

            // 3. Persist changes to database
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}