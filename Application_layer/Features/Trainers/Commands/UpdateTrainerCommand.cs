using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Titan_Fitness.Application_layer.DTOS;
using Titan_Fitness.Domain.Entites;
using Titan_Fitness.Domain.Interfaces;
using Titan_Fitness.Domain.Value_object;

namespace Titan_Fitness.Application_layer.Features.Trainers.Commands
{
    // Command Definition
    public record UpdateTrainerCommand(int Id, CreateTrainerDto TrainerDto) : IRequest;

    // Handler Implementation
    public class UpdateTrainerCommandHandler : IRequestHandler<UpdateTrainerCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTrainerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(UpdateTrainerCommand request, CancellationToken cancellationToken)
        {
            if (request.TrainerDto == null)
            {
                throw new ArgumentNullException(nameof(request.TrainerDto), "بيانات التعديل مطلوبة.");
            }

            // 1. Fetch trainer from repository
            var trainer = await _unitOfWork.Trainers.GetByIdAsync(request.Id, cancellationToken);

            if (trainer == null)
            {
                throw new KeyNotFoundException($"المدرب برقم {request.Id} غير موجود.");
            }

            var dto = request.TrainerDto;

            // 2. Create Phone Value Object
            var phone = Phone.Create(dto.Phone ?? string.Empty);

            // 3. Update entity state using Domain Method (UpdateProfile)
            trainer.UpdateProfile(
                name: dto.FullName,
                email: dto.Email,
                phone: phone
            );

            // 4. Update status if changed
            if (dto.IsActive)
            {
                trainer.Activate();
            }
            else
            {
                trainer.Deactivate();
            }

            // 5. Persist changes
            _unitOfWork.Trainers.Update(trainer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}